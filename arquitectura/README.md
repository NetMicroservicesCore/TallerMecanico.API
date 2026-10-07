# Arquitectura base e interacciones

## Patrón elegido

Monolito modular con arquitectura limpia por capas y puertos/adaptadores. Un despliegue inicial mantiene la operación simple y las transacciones locales. El módulo Clientes tiene contratos propios; nuevos módulos (vehículos, órdenes, inventario) deben añadir sus casos de uso sin colocar lógica en controladores. La separación permite evolucionar a servicios independientes cuando existan necesidades de escala y límites de negocio comprobados. Esta entrega no afirma tener una arquitectura perfecta ni capacidad de tráfico medida.

Dependencias de compilación: `Application → Domain`, `Infrastructure → Application → Domain`, `API.Principal → Infrastructure` (raíz de composición) y `Taller.Tests → API.Principal`. El controlador utiliza Application; el servicio depende del puerto `IClienteRepository`, cuya implementación se registra por inyección de dependencias. No hay acceso directo a SQL desde HTTP ni dependencias EF en el dominio.

## Componentes

| Componente | Responsabilidad | Interacción |
|---|---|---|
| Program / pipeline | Configuración, DI, HTTPS, errores, límites, JWT y autorización | Prepara contexto, autentica y autoriza antes de MVC |
| ApiExceptionHandler | Respuesta 500 genérica y log de tipo/correlación | No devuelve excepciones, SQL ni información personal |
| ClientesController | Model binding, validación, estados HTTP y wrappers | Invoca ClienteService; no consulta DbContext |
| RequestWrapper | Contenedor `data` obligatorio | Permite validación recursiva del DTO de entrada |
| ResponseWrapper | `HttpStatusCode`, mensaje y datos tipados | Serializa códigos numéricos iguales al estado HTTP |
| CrearClienteRequest / ClientesQuery | Campos admitidos y restricciones | Evita asignar Id/fecha desde el cuerpo; orden por allowlist |
| ClienteService | Valida también fuera de MVC, normaliza bordes y coordina | Crea Cliente, llama al puerto y devuelve DTO |
| IClienteRepository | Puerto específico de alta y consulta | No filtra IQueryable ni tipos de EF a Application |
| ClienteRepository | LINQ traducido a SQL, proyección y paginación | Usa DbContext; las consultas no cargan toda la tabla |
| TallerDbContext | Mapeo, seguimiento de escritura y unidad de trabajo | SaveChangesAsync confirma una transacción local |
| Cliente / ClienteDto | Estado interno y contrato público separados | Incluye dirección y contacto; no son usuarios autenticables |
| InitialClientes / snapshot | Evolución reproducible del esquema | Tabla, longitudes e índice compuesto Nombre/Id |
| TallerDbContextFactory | Activación de herramientas EF | Permite migrar sin iniciar HTTP ni usar el IdP |
| SQL Server | Persistencia y orden/cotejamiento | Índice compuesto para orden por nombre e Id |
| Taller.Tests | Pruebas HTTP, JWT real y SQL real | Base temporal y claves RSA efímeras; no requiere IdP externo |

El diagrama representa el camino lógico del repositorio a SQL Server; la ejecución SQL pasa por TallerDbContext y el proveedor EF. Los componentes son módulos dentro del mismo proceso, no microservicios desplegados. El contenido del diagrama está en español; los controles fijos del visor Archify y su atributo HTML de idioma usan inglés, ya que el visor no incluye localización española.

## Flujo GET

1. El cliente envía HTTPS, Bearer y parámetros query; no se utiliza cuerpo en GET.
2. El pipeline asigna correlación, aplica límites y verifica emisor, audiencia, firma y vigencia del token.
3. La política exige `permission=clientes.manage`. MVC valida página, tamaño y dirección.
4. El controlador llama a ClienteService, que valida y delega en IClienteRepository.
5. El repositorio ejecuta un COUNT SQL y una consulta `ORDER BY Nombre, Id` con OFFSET/FETCH, proyección DTO y `AsNoTracking`. Para desc, invierte ambos campos.
6. Se devuelve 200 con PagedResponse dentro de ResponseWrapper. Una página sin filas es un éxito con `items: []`.

Los empates son deterministas mientras el conjunto no cambia. Conteo y página son dos consultas sin snapshot compartido: cambios concurrentes pueden alterar total/filas. OFFSET puede desplazar filas entre páginas con escrituras concurrentes. Para navegación profunda o volumen grande, evolucionar a cursor compuesto Nombre/Id y medir los planes reales. El orden cultural depende del cotejamiento SQL del entorno; no se fuerza un orden de .NET en memoria.

## Flujo POST

1. Se exigen HTTPS, Bearer, permiso y contenido JSON.
2. MVC valida `data` y sus campos antes de ejecutar el controlador.
3. ClienteService vuelve a validar para proteger invocaciones no HTTP, elimina espacios de los extremos y construye Cliente.
4. Id y fecha UTC se generan en servidor; el cliente no puede asignarlos.
5. El repositorio registra la entidad en EF y confirma SaveChangesAsync. La escritura no tiene reintentos automáticos que puedan duplicar un alta.
6. El servicio proyecta a ClienteDto; el controlador devuelve 201 con datos persistidos. No se publica un Location a un endpoint por Id que no existe.

No se impone unicidad del email porque no se solicitó y varios clientes podrían compartirlo. Repetir el POST crea otro cliente. Si el negocio exige deduplicación o reintentos seguros, definir la clave de negocio o agregar una clave de idempotencia persistida con restricción única; no basta una búsqueda previa.

## Interpretación de las referencias

- `Especifications/api.md` contiene ejemplos de cursos mezclados con clientes: se conservaron su base versionada y política, pero prevalecen los campos y dos operaciones solicitados. No se implementaron los endpoints ajenos de cursos, login, PUT ni DELETE.
- `data-skills.md` guía el desarrollo manual, EF Core, migraciones y pruebas; no se usó scaffolding CLI. Su sugerencia de CRUD completo se acota a los dos verbos expresamente pedidos.
- El ejemplo del usuario repite ResponseWrapper para entrada y salida: se separó RequestWrapper porque estado HTTP y mensaje de resultado pertenecen al servidor.
- `colonia` se almacena una sola vez. Dirección se mantiene en columnas de Cliente mientras no tenga ciclo de vida propio.
- Apellido materno es opcional; los demás campos solicitados son obligatorios. Código postal es texto de cinco dígitos para México, preservando ceros. Teléfonos admiten 7–25 caracteres con números, espacios, paréntesis, guion y prefijo `+`. Son reglas iniciales que deben ampliarse si se atienden direcciones de otros países.

## Seguridad y operación

- JWT Bearer obtenido de un emisor OIDC HTTPS; no se crean contraseñas ni claves de firma dentro de la API. Política aplicada a los dos verbos de /api/v1/clientes; 401 y 403 diferenciados.
- Model binding sobre DTOs, consultas parametrizadas de EF, límites de longitud, 32 KiB de cuerpo en Kestrel y cancelación hasta SQL. Fallos de infraestructura devuelven 500 genérico, no detalles internos.
- `X-Correlation-ID` se basa en la traza del servidor; no se copia una cabecera arbitraria. No se registran cuerpos, tokens ni valores personales. Se suprime el logger predeterminado de excepciones HTTP para evitar que vuelva a registrar el detalle sensible; el manejador registra tipo y correlación. Al habilitar APM, revisar su captura automática de excepciones.
- Respuestas con `Cache-Control: no-store` y `nosniff`; HSTS fuera de Development. OpenAPI y Swagger UI solamente en Development, con esquema Bearer y operationId distintos para GET y POST de v1. La raíz redirige a Swagger; no se publica el alias sin versión.
- Límite inicial global de 64 solicitudes concurrentes y 600 por minuto POR INSTANCIA, sin cola. Son presupuestos conservadores configurados en código, no una garantía de throughput ni cuotas por usuario. Complementar con límites por consumidor en un gateway y dimensionarlos con mediciones.
- No se habilita CORS por defecto. Agregar solo orígenes conocidos si una SPA necesita acceso cruzado.
- Producción exige `ConnectionStrings__Taller`, `Authentication__Authority`, `Authentication__Audience` y `AllowedHosts`. Usar un gestor de secretos, cuenta SQL de mínimo privilegio, cifrado con certificado validado, respaldos y política de retención de datos personales. `TrustServerCertificate=True` se limita al ejemplo LocalDB.
- No se aceptan forwarded headers por defecto. Si TLS termina en un proxy, configurar proxies/redes confiables y middleware de forwarded headers antes de HTTPS; no confiar en cabeceras de cualquier origen. Alternativamente mantener HTTPS hasta Kestrel. La topología final depende del proveedor cloud.
- Migraciones separadas del arranque, aplicadas por un proceso de despliegue único con credenciales de DDL. Las réplicas HTTP pueden escalar sin estado local de sesión. No se configuraron autoscaling, WAF, Redis, colas, métricas exportadas, readiness ni infraestructura cloud en esta entrega.
- No se cachea el listado de PII. Antes de introducir caché o CQRS, medir latencia p95/p99, errores, concurrencia, CPU, pool SQL, planes e I/O con datos representativos. Las pruebas funcionales no sustituyen pruebas de carga.

## Extensión de módulos

Crear entidad/contratos y un servicio de aplicación por caso de uso, agregar un puerto solo cuando haya dependencia externa, implementar adaptadores y registrar en DI. Reutilizar wrappers y pipeline. Añadir autorización específica por módulo, migración y pruebas de integración. Evitar un repositorio genérico que exponga IQueryable o un controlador base con reglas de negocio. EF ya ofrece una unidad de trabajo; no se agrega otra sin necesidad.

## Referencias técnicas

- [Consultas eficientes de EF Core](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying): proyección, índices y costos de paginación.
- [JWT Bearer en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0): validación y configuración del emisor.
- [Rate limiting en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-9.0): límites locales y necesidad de pruebas de carga.
