# API base del taller mecánico

Implementación en ASP.NET Core 9 y EF Core 9 con SQL Server. La solución conserva el proyecto API.Principal y agrega capas separadas con dependencias verificables por compilación.

- [Arquitectura y decisiones](arquitectura/README.md)
- [Diagrama interactivo Archify](arquitectura/clientes.html)
- [Contrato y ejemplos HTTP](API.Principal/Clientes.http)
- [Documentación por línea de C#](arquitectura/codigo-linea-por-linea.md)
- [Evidencia de validación](arquitectura/validacion.md)

## Ejecutar localmente

Desde esta carpeta, en PowerShell, con SDK 9.0.318 y SQL Server LocalDB:

```powershell
dotnet tool restore
dotnet restore TallerMecanico.API.sln
dotnet build TallerMecanico.API.sln -m:1
dotnet ef database update --project Taller.Infrastructure --startup-project API.Principal
dotnet test Taller.Tests/Taller.Tests.csproj -m:1
dotnet run --project API.Principal --launch-profile https
```

El certificado HTTPS de desarrollo debe estar confiado en el equipo. Swagger UI: `https://localhost:7230/swagger/index.html`. OpenAPI: `https://localhost:7230/openapi/v1.json`. En Development, `/` redirige a Swagger. La migración crea `TallerMecanicoDev` en LocalDB; el inicio de la API nunca modifica el esquema. Las pruebas crean y eliminan únicamente una base `TallerTests_<uuid>` independiente.

**Autenticación:** la URL `https://identity.example.invalid` es un marcador no funcional. Configura tu emisor OIDC y audiencia mediante variables de entorno o user-secrets antes de enviar un token real:

```powershell
$env:Authentication__Authority = 'https://TU-EMISOR-OIDC'
$env:Authentication__Audience = 'taller-api'
# Para una instancia distinta de LocalDB, proporciona una conexión cifrada mediante un gestor de secretos.
# ConnectionStrings__Taller reemplaza la cadena local.
```

El emisor debe incluir `permission` con valor `clientes.manage` en el access token. No se implementó registro/login de usuarios: requiere definir el proveedor de identidad. No existe bypass anónimo de desarrollo. Las pruebas firman tokens RSA efímeros y configuran únicamente su host aislado; no hay llaves de prueba en la API.

## Endpoints

| Método | Ruta | Resultado |
|---|---|---|
| GET | `/api/v1/clientes?pageNumber=1&pageSize=20&sortDirection=asc` | 200, página de clientes |
| POST | `/api/v1/clientes` | 201, cliente persistido |

La versión es obligatoria: `/api/clientes` y `/api/v2/clientes` devuelven 404. `sortDirection` acepta exactamente `asc` o `desc`; `pageNumber` va de 1 a 10000 y `pageSize` de 1 a 100. Ambos endpoints exigen autorización.

Entrada POST: `{ "data": { ...campos del cliente... } }`. Salida: `{ "statusCode": 201, "message": "Cliente registrado.", "data": { ...cliente... } }`. GET devuelve en `data`: `items`, `pageNumber`, `pageSize`, `totalCount`, `totalPages`, `hasNextPage`, `hasPreviousPage`. Los errores usan el mismo sobre, salvo rechazos del servidor/proxy anteriores al pipeline de aplicación.

El archivo `Clientes.http` contiene ejemplos completos. Su ejecución manual requiere el token real; los escenarios equivalentes se ejecutan automáticamente en `Taller.Tests` contra el pipeline ASP.NET y SQL Server.

## Contenedor

Desde la raíz de la solución: `docker build -f API.Principal/Dockerfile -t taller-api .`. El Dockerfile copia las cuatro capas antes de restaurar y ejecuta como usuario sin privilegios. La imagen no incluye SQL Server ni el proveedor de identidad. No se verificó Docker en esta entrega. Configura conexión, emisor, audiencia, hosts permitidos y certificado HTTPS mediante la plataforma de despliegue; `EXPOSE` no configura TLS. LocalDB es solo para Windows local.
