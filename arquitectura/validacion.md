# Evidencia de validación

Fecha local: 2026-10-07. SDK utilizado: 9.0.318; objetivo net9.0; EF Core SQL Server 9.0.20.

## API y persistencia

- Compilación de API y dependencias: correcta, 0 advertencias y 0 errores.
- `dotnet test Taller.Tests/Taller.Tests.csproj -m:1`: **23 correctas, 0 fallidas, 0 omitidas**.
- Las pruebas ejecutan el pipeline HTTP ASP.NET real mediante TestServer y el proveedor SQL Server real en LocalDB. No usan EF InMemory ni repositorio simulado para el flujo funcional. Solo el caso negativo de 500 sustituye el puerto con un fallo deliberado.
- JWT: firma RSA real con clave efímera; se probaron permiso ausente, firma incorrecta, expiración y audiencia incorrecta. Descubrimiento OIDC sustituido por configuración estática únicamente en el host de pruebas.
- Migración `20261007183209_InitialClientes` aplicada correctamente tanto en bases temporales de pruebas como en `TallerMecanicoDev` local.
- `dotnet ef migrations has-pending-model-changes --project Taller.Infrastructure --startup-project API.Principal`: sin cambios pendientes.
- Reporte TRX reproducible: `Taller.Tests/TestResults/clientes.trx` (salida local ignorada por Git).

| Solicitud / caso | Resultado verificado |
|---|---|
| POST /api/clientes autorizado, 4 registros de prueba | 201; DTO, UUID, código postal y correlación |
| GET /api/v1/clientes, páginas 1 y 2, asc | 200; total 4; páginas de 2; sin IDs repetidos |
| GET /api/clientes desc | 200; orden e IDs inversos, incluidos empates por nombre |
| GET /api/clientes fuera de las páginas con datos | 200; items vacío |
| GET y POST sin token en ambos alias | 401 con wrapper |
| Token sin permiso | 403 |
| Token expirado, audiencia incorrecta, firma incorrecta | 401 |
| Página/tamaño/orden fuera de contrato | 400 |
| Correo inválido, data ausente/null/vacía, JSON malformado | 400 |
| POST text/plain | 415 con wrapper |
| Repositorio falla con mensaje sensible deliberado | 500; detalle no expuesto |
| Más de 600 solicitudes en la ventana del host aislado | 429 con wrapper |
| GET /openapi/v1.json | Ambas rutas, verbos y seguridad Bearer documentados |

Los archivos `.http` son ejemplos ejecutables con un token real; se verificaron sus escenarios por TestServer, no contra un emisor OIDC productivo. Las bases de pruebas se eliminaron mediante una guarda que exige el nombre exacto aleatorio creado por el fixture. TallerMecanicoDev conserva solamente el esquema, sin datos de prueba sembrados.

## Diagrama Archify

- Tipo: `architecture`.
- Archivo: `clientes.html`; especificación: `clientes.architecture.json`.
- Validación: **9/9 showcase, 0 errores, 0 advertencias**.
- `browser_evidence: passed`: Chrome midió ausencia de overflow en 1440×900, 1600×1000, 1920×1080 y 2048×1320.
- `visual_review: passed`: revisión de capturas de 2048×1320 claro y 1440×900 oscuro; componentes, etiquetas y tarjetas legibles, sin cruces visibles. La comprobación automática también capturó los temas inversos.
- `correction_rounds: 1`: se compactó el espacio vertical tras detectar overflow en la primera entrega al navegador.
- SHA-256 especificación: `9c7c969d23a8431e3d2be265d33486229d8b02abab57391b5ded158c2ec8fc1f`.
- SHA-256 HTML: `c772d2fe54dc256718437c601683c04667526a663d8b27d9ade0b3483de0e2a1`.
- Recibos: `archify-delivery.json` y `clientes.visual-check.json`; capturas y contact sheet junto al HTML.
- Contenido del diagrama en español; controles del visor y atributo HTML de idioma en inglés por limitación de Archify.

## Límites de esta evidencia

No se midieron rendimiento, carga, autoscaling ni latencia sobre infraestructura cloud. No se verificaron el emisor OIDC real, un SQL Server remoto, Docker ni terminación TLS en proxy. Son configuraciones de despliegue que necesitan el entorno objetivo; la validación local no las sustituye.

## Prueba adicional de servidor HTTPS

Se inició temporalmente Kestrel con el perfil `https` y se detuvo después de comprobar:

- `GET https://localhost:7230/openapi/v1.json`: **200**.
- `GET https://localhost:7230/api/clientes` sin token: **401**, sobre JSON consistente y `X-Correlation-ID` presente.

La sonda omitió solamente la validación de confianza del certificado local de desarrollo (`SkipCertificateCheck`); el tráfico usó HTTPS. No se modificó la configuración TLS del servidor. Esto no acredita la confianza del certificado ni una integración con el emisor real.
