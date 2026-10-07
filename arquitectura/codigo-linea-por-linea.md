# Documentación por línea de C#

Cada fila corresponde a una línea no vacía del código entregado. Los comentarios de intención están junto al código; este índice también explica delimitadores y las migraciones generadas por EF. Las líneas vacías solo separan bloques. Los archivos bin/obj no forman parte del código fuente entregado.

## API.Principal\Contracts\Wrappers.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.ComponentModel.DataAnnotations; // Validación del cuerpo exterior.` | Validación del cuerpo exterior. |
| 2 | `using System.Net; // Código HTTP fuertemente tipado.` | Código HTTP fuertemente tipado. |
| 3 | `namespace API.Principal.Contracts; // Contratos exclusivos del transporte HTTP.` | Contratos exclusivos del transporte HTTP. |
| 5 | `/// <summary>La entrada solo contiene datos; el consumidor no decide el estado HTTP.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 6 | `public sealed class RequestWrapper<T> where T : class` | Declara el tipo y sus dependencias o clase base. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `[Required] // Rechaza cuerpos sin la propiedad data o con data null.` | Rechaza cuerpos sin la propiedad data o con data null. |
| 9 | `public required T Data { get; init; } // ASP.NET también valida el objeto anidado.` | ASP.NET también valida el objeto anidado. |
| 10 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 12 | `/// <summary>Sobre uniforme; StatusCode se serializa como número y coincide con HTTP.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 13 | `public sealed record ResponseWrapper<T>(HttpStatusCode StatusCode, string Message, T? Data); // Modelo inmutable.` | Modelo inmutable. |

## API.Principal\Controllers\ClientesController.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.Net; // Estados HTTP del sobre.` | Estados HTTP del sobre. |
| 2 | `using API.Principal.Contracts; // Wrappers de transporte.` | Wrappers de transporte. |
| 3 | `using Microsoft.AspNetCore.Authorization; // Políticas.` | Políticas. |
| 4 | `using Microsoft.AspNetCore.Mvc; // Contratos HTTP y OpenAPI.` | Contratos HTTP y OpenAPI. |
| 5 | `using Taller.Application.Clientes; // Casos de uso.` | Casos de uso. |
| 6 | `namespace API.Principal.Controllers; // Adaptador HTTP.` | Adaptador HTTP. |
| 7 | `/// <summary>Registro y listado paginado de clientes.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 8 | `[ApiController] // Valida automáticamente cuerpo y query.` | Valida automáticamente cuerpo y query. |
| 9 | `[Route("api/clientes")] // Ruta solicitada.` | Ruta solicitada. |
| 10 | `[Route("api/v1/clientes")] // Alias compatible con api.md.` | Alias compatible con api.md. |
| 11 | `[Authorize(Policy = "CanManageClientes")] // Exige token y permiso.` | Exige token y permiso. |
| 12 | `[Produces("application/json")] // Contrato JSON.` | Contrato JSON. |
| 13 | `[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // Protege datos personales.` | Protege datos personales. |
| 14 | `[ProducesResponseType<ResponseWrapper<object>>(400)] // Entrada inválida.` | Entrada inválida. |
| 15 | `[ProducesResponseType<ResponseWrapper<object>>(401)] // Sin autenticación.` | Sin autenticación. |
| 16 | `[ProducesResponseType<ResponseWrapper<object>>(403)] // Sin permiso.` | Sin permiso. |
| 17 | `[ProducesResponseType<ResponseWrapper<object>>(429)] // Saturación.` | Saturación. |
| 18 | `[ProducesResponseType<ResponseWrapper<object>>(500)] // Fallo sanitizado.` | Fallo sanitizado. |
| 19 | `public sealed class ClientesController(ClienteService service) : ControllerBase` | Declara el tipo y sus dependencias o clase base. |
| 20 | `{` | Abre el bloque de definición o inicialización precedente. |
| 21 | `/// <summary>Ordena por nombre e Id con paginación acotada.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 22 | `[HttpGet] // Consulta sin cuerpo.` | Consulta sin cuerpo. |
| 23 | `[EndpointSummary("Consultar clientes paginados")] // Documentación OpenAPI.` | Documentación OpenAPI. |
| 24 | `[ProducesResponseType<ResponseWrapper<PagedResponse<ClienteDto>>>(200)] // Contrato de éxito.` | Contrato de éxito. |
| 25 | `public async Task<IActionResult> Get([FromQuery] ClientesQuery query, CancellationToken cancellationToken)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 26 | `{` | Abre el bloque de definición o inicialización precedente. |
| 27 | `var page = await service.ListAsync(query, cancellationToken); // Propaga cancelación.` | Propaga cancelación. |
| 28 | `return Ok(new ResponseWrapper<PagedResponse<ClienteDto>>(HttpStatusCode.OK, "Clientes consultados.", page)); // Sobre y HTTP 200.` | Sobre y HTTP 200. |
| 29 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 30 | `/// <summary>Valida y registra un cliente.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 31 | `[HttpPost] // Alta explícita.` | Alta explícita. |
| 32 | `[Consumes("application/json")] // Rechaza otros formatos.` | Rechaza otros formatos. |
| 33 | `[EndpointSummary("Registrar cliente")] // Documentación OpenAPI.` | Documentación OpenAPI. |
| 34 | `[ProducesResponseType<ResponseWrapper<ClienteDto>>(201)] // Alta confirmada.` | Alta confirmada. |
| 35 | `[ProducesResponseType<ResponseWrapper<object>>(415)] // Formato no soportado.` | Formato no soportado. |
| 36 | `public async Task<IActionResult> Post([FromBody] RequestWrapper<CrearClienteRequest> request, CancellationToken cancellationToken)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 37 | `{` | Abre el bloque de definición o inicialización precedente. |
| 38 | `var cliente = await service.CreateAsync(request.Data, cancellationToken); // Delega persistencia.` | Delega persistencia. |
| 39 | `return StatusCode(201, new ResponseWrapper<ClienteDto>(HttpStatusCode.Created, "Cliente registrado.", cliente)); // No inventa un GET por Id inexistente.` | No inventa un GET por Id inexistente. |
| 40 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 41 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## API.Principal\Middleware\ApiExceptionHandler.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.Net; // Estados de error.` | Estados de error. |
| 2 | `using API.Principal.Contracts; // Sobre uniforme.` | Sobre uniforme. |
| 3 | `using Microsoft.AspNetCore.Diagnostics; // Manejo global.` | Manejo global. |
| 4 | `namespace API.Principal.Middleware; // Responsabilidades transversales.` | Responsabilidades transversales. |
| 5 | `/// <summary>Oculta detalles internos y evita registrar datos personales.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 6 | `public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler` | Declara el tipo y sus dependencias o clase base. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 9 | `{` | Abre el bloque de definición o inicialización precedente. |
| 10 | `logger.LogError("Fallo {ExceptionType}; correlación {TraceId}", exception.GetType().Name, context.TraceIdentifier); // Solo tipo y correlación.` | Solo tipo y correlación. |
| 11 | `context.Response.StatusCode = 500; // Estado real del error.` | Estado real del error. |
| 12 | `await context.Response.WriteAsJsonAsync(new ResponseWrapper<object>(HttpStatusCode.InternalServerError,` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 13 | `"Ocurrió un error interno. Usa X-Correlation-ID para solicitar soporte.", null), cancellationToken); // Sin stack trace ni SQL.` | Sin stack trace ni SQL. |
| 14 | `return true; // Marca la excepción como manejada.` | Marca la excepción como manejada. |
| 15 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 16 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## API.Principal\Program.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.Diagnostics; // Trazas distribuidas.` | Trazas distribuidas. |
| 2 | `using System.Net; // Estados HTTP.` | Estados HTTP. |
| 3 | `using System.Threading.RateLimiting; // Protección de recursos.` | Protección de recursos. |
| 4 | `using API.Principal.Contracts; // Wrappers.` | Wrappers. |
| 5 | `using API.Principal.Middleware; // Errores globales.` | Errores globales. |
| 6 | `using Microsoft.AspNetCore.Authentication.JwtBearer; // Autenticación JWT.` | Autenticación JWT. |
| 7 | `using Microsoft.AspNetCore.Mvc; // Validación MVC.` | Validación MVC. |
| 8 | `using Microsoft.EntityFrameworkCore; // Persistencia EF.` | Persistencia EF. |
| 9 | `using Microsoft.OpenApi.Models; // Documenta autenticación Bearer y operaciones únicas.` | Documenta autenticación Bearer y operaciones únicas. |
| 10 | `using Taller.Application.Clientes; // Casos de uso.` | Casos de uso. |
| 11 | `using Taller.Infrastructure; // Adaptadores.` | Adaptadores. |
| 13 | `var builder = WebApplication.CreateBuilder(args); // Carga configuración externa.` | Carga configuración externa. |
| 14 | `builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 32 * 1024); // Limita tamaño del cuerpo.` | Limita tamaño del cuerpo. |
| 15 | `var connection = builder.Configuration.GetConnectionString("Taller") // Cadena externa, sin secretos versionados.` | Cadena externa, sin secretos versionados. |
| 16 | `?? throw new InvalidOperationException("Configura ConnectionStrings:Taller."); // Falla temprano.` | Falla temprano. |
| 17 | `var authority = builder.Configuration["Authentication:Authority"]; // Emisor OIDC.` | Emisor OIDC. |
| 18 | `var audience = builder.Configuration["Authentication:Audience"]; // Destinatario esperado.` | Destinatario esperado. |
| 19 | `if (!Uri.TryCreate(authority, UriKind.Absolute, out var issuer) &#124;&#124; issuer.Scheme != "https" &#124;&#124; string.IsNullOrWhiteSpace(audience)) // Impide configuración insegura.` | Impide configuración insegura. |
| 20 | `throw new InvalidOperationException("Configura Authentication:Authority HTTPS y Authentication:Audience."); // No permite acceso anónimo accidental.` | No permite acceso anónimo accidental. |
| 21 | `builder.Services.AddDbContext<TallerDbContext>(options => options.UseSqlServer(connection, sql => sql.CommandTimeout(30))); // Contexto scoped y timeout SQL.` | Contexto scoped y timeout SQL. |
| 22 | `builder.Services.AddScoped<IClienteRepository, ClienteRepository>(); // Inversión de dependencia.` | Inversión de dependencia. |
| 23 | `builder.Services.AddScoped<ClienteService>(); // Servicio por solicitud.` | Servicio por solicitud. |
| 24 | `builder.Services.AddExceptionHandler<ApiExceptionHandler>(); // Errores sanitizados.` | Errores sanitizados. |
| 25 | `builder.Services.AddProblemDetails(); // Infraestructura del manejador global.` | Infraestructura del manejador global. |
| 26 | `builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => // Personaliza validación automática.` | Personaliza validación automática. |
| 27 | `{` | Abre el bloque de definición o inicialización precedente. |
| 28 | `options.SuppressMapClientErrors = true; // Evita otro formato de errores.` | Evita otro formato de errores. |
| 29 | `options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult( // Conserva HTTP 400.` | Conserva HTTP 400. |
| 30 | `new ResponseWrapper<object>(HttpStatusCode.BadRequest, "Solicitud inválida.",` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 31 | `context.ModelState.Where(x => x.Value?.Errors.Count > 0).ToDictionary(x => x.Key, _ => new[] { "Valor inválido o requerido." }))); // No refleja valores ni errores del parser.` | No refleja valores ni errores del parser. |
| 32 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 33 | `builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => // Valida access tokens externos.` | Valida access tokens externos. |
| 34 | `{` | Abre el bloque de definición o inicialización precedente. |
| 35 | `options.Authority = authority; // Descubrimiento de claves públicas.` | Descubrimiento de claves públicas. |
| 36 | `options.Audience = audience; // Token dirigido a esta API.` | Token dirigido a esta API. |
| 37 | `options.RequireHttpsMetadata = true; // Solo descubrimiento cifrado.` | Solo descubrimiento cifrado. |
| 38 | `options.MapInboundClaims = false; // Conserva nombres originales.` | Conserva nombres originales. |
| 39 | `options.IncludeErrorDetails = false; // No expone detalles criptográficos.` | No expone detalles criptográficos. |
| 40 | `options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30); // Tolerancia temporal acotada.` | Tolerancia temporal acotada. |
| 41 | `options.TokenValidationParameters.ValidateIssuer = true; // Valida emisor.` | Valida emisor. |
| 42 | `options.TokenValidationParameters.ValidateAudience = true; // Valida destinatario.` | Valida destinatario. |
| 43 | `options.TokenValidationParameters.ValidateLifetime = true; // Rechaza expiración.` | Rechaza expiración. |
| 44 | `options.TokenValidationParameters.RequireSignedTokens = true; // Rechaza tokens sin firma.` | Rechaza tokens sin firma. |
| 45 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 46 | `builder.Services.AddAuthorization(options => options.AddPolicy("CanManageClientes", policy => // Política especificada.` | Política especificada. |
| 47 | `policy.RequireAuthenticatedUser().RequireClaim("permission", "clientes.manage"))); // Permiso explícito.` | Permiso explícito. |
| 48 | `builder.Services.AddRateLimiter(options => // Límites por instancia; complementar en gateway distribuido.` | Límites por instancia; complementar en gateway distribuido. |
| 49 | `{` | Abre el bloque de definición o inicialización precedente. |
| 50 | `options.GlobalLimiter = PartitionedRateLimiter.CreateChained( // Presupuesto de concurrencia y frecuencia.` | Presupuesto de concurrencia y frecuencia. |
| 51 | `PartitionedRateLimiter.Create<HttpContext, string>(_ => RateLimitPartition.GetConcurrencyLimiter("concurrency", _ => // Clave fija evita cardinalidad ilimitada.` | Clave fija evita cardinalidad ilimitada. |
| 52 | `new ConcurrencyLimiterOptions { PermitLimit = 64, QueueLimit = 0 })), // Sin cola de saturación.` | Sin cola de saturación. |
| 53 | `PartitionedRateLimiter.Create<HttpContext, string>(_ => RateLimitPartition.GetFixedWindowLimiter("requests", _ => // Límite agregado.` | Límite agregado. |
| 54 | `new FixedWindowRateLimiterOptions { PermitLimit = 600, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }))); // Ajustar con mediciones.` | Ajustar con mediciones. |
| 55 | `options.OnRejected = async (context, token) => // Error JSON uniforme.` | Error JSON uniforme. |
| 56 | `{` | Abre el bloque de definición o inicialización precedente. |
| 57 | `context.HttpContext.Response.StatusCode = 429; // Demasiadas solicitudes.` | Demasiadas solicitudes. |
| 58 | `await context.HttpContext.Response.WriteAsJsonAsync(new ResponseWrapper<object>((HttpStatusCode)429, "Límite de solicitudes excedido.", null), token); // Sin datos internos.` | Sin datos internos. |
| 59 | `};` | Cierra el bloque, inicialización o llamada precedente. |
| 60 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 61 | `builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, context, token) => // Enriquece el contrato sin paquetes adicionales.` | Enriquece el contrato sin paquetes adicionales. |
| 62 | `{` | Abre el bloque de definición o inicialización precedente. |
| 63 | `document.Components ??= new OpenApiComponents(); // Inicializa componentes si no hay esquemas.` | Inicializa componentes si no hay esquemas. |
| 64 | `document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme // Access token del proveedor externo.` | Access token del proveedor externo. |
| 65 | `{` | Abre el bloque de definición o inicialización precedente. |
| 66 | `Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", // Esquema estándar HTTP.` | Esquema estándar HTTP. |
| 67 | `Description = "Access token con claim permission=clientes.manage." // Explica el permiso necesario.` | Explica el permiso necesario. |
| 68 | `};` | Cierra el bloque, inicialización o llamada precedente. |
| 69 | `foreach (var path in document.Paths.Where(x => x.Key.EndsWith("/clientes", StringComparison.Ordinal))) // Documenta ambos alias del recurso.` | Documenta ambos alias del recurso. |
| 70 | `foreach (var operation in path.Value.Operations) // GET y POST.` | GET y POST. |
| 71 | `{` | Abre el bloque de definición o inicialización precedente. |
| 72 | `operation.Value.OperationId = operation.Key + (path.Key.Contains("/v1/") ? "ClientesV1" : "Clientes"); // Identificadores únicos para generadores de clientes.` | Identificadores únicos para generadores de clientes. |
| 73 | `operation.Value.Security = [new OpenApiSecurityRequirement // Exige Bearer en documentación.` | Exige Bearer en documentación. |
| 74 | `{` | Abre el bloque de definición o inicialización precedente. |
| 75 | `[new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>() // HTTP Bearer no utiliza scopes OpenAPI.` | HTTP Bearer no utiliza scopes OpenAPI. |
| 76 | `}];` | Cierra el bloque, inicialización o llamada precedente. |
| 77 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 78 | `return Task.CompletedTask; // Transformación local sin I/O.` | Transformación local sin I/O. |
| 79 | `}));` | Cierra el bloque, inicialización o llamada precedente. |
| 80 | `var app = builder.Build(); // Construye el servidor.` | Construye el servidor. |
| 81 | `app.Use(async (context, next) => // Correlación controlada por el servidor.` | Correlación controlada por el servidor. |
| 82 | `{` | Abre el bloque de definición o inicialización precedente. |
| 83 | `context.TraceIdentifier = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N"); // No confía en cabeceras arbitrarias.` | No confía en cabeceras arbitrarias. |
| 84 | `context.Response.Headers["X-Correlation-ID"] = context.TraceIdentifier; // Identifica la solicitud.` | Identifica la solicitud. |
| 85 | `context.Response.Headers["X-Content-Type-Options"] = "nosniff"; // Impide interpretación incorrecta del contenido.` | Impide interpretación incorrecta del contenido. |
| 86 | `context.Response.Headers.CacheControl = "no-store"; // Protege datos personales.` | Protege datos personales. |
| 87 | `await next(context); // Continúa el pipeline.` | Continúa el pipeline. |
| 88 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 89 | `app.UseExceptionHandler(); // Captura errores del pipeline siguiente.` | Captura errores del pipeline siguiente. |
| 90 | `app.UseStatusCodePages(async context => // Completa errores sin cuerpo, incluidos 401 y 403.` | Completa errores sin cuerpo, incluidos 401 y 403. |
| 91 | `{` | Abre el bloque de definición o inicialización precedente. |
| 92 | `var response = context.HttpContext.Response; // Estado determinado por ASP.NET.` | Estado determinado por ASP.NET. |
| 93 | `await response.WriteAsJsonAsync(new ResponseWrapper<object>((HttpStatusCode)response.StatusCode, "La solicitud no pudo completarse.", null)); // Alinea sobre y HTTP.` | Alinea sobre y HTTP. |
| 94 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 95 | `if (!app.Environment.IsDevelopment()) app.UseHsts(); // HSTS fuera de desarrollo.` | HSTS fuera de desarrollo. |
| 96 | `app.UseHttpsRedirection(); // Exige transporte cifrado al cliente.` | Exige transporte cifrado al cliente. |
| 97 | `app.UseRouting(); // Resuelve endpoint.` | Resuelve endpoint. |
| 98 | `app.UseRateLimiter(); // Protege incluso antes del trabajo criptográfico.` | Protege incluso antes del trabajo criptográfico. |
| 99 | `app.UseAuthentication(); // Valida identidad.` | Valida identidad. |
| 100 | `app.UseAuthorization(); // Evalúa permisos.` | Evalúa permisos. |
| 101 | `if (app.Environment.IsDevelopment()) app.MapOpenApi(); // Solo publica documentación en desarrollo.` | Solo publica documentación en desarrollo. |
| 102 | `app.MapControllers(); // Publica clientes.` | Publica clientes. |
| 103 | `app.Run(); // No migra ni siembra automáticamente.` | No migra ni siembra automáticamente. |
| 104 | `public partial class Program { } // Permite pruebas de integración con WebApplicationFactory.` | Permite pruebas de integración con WebApplicationFactory. |

## Taller.Application\Clientes\ClienteDto.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.Linq.Expressions; // Proyección traducible a SQL.` | Proyección traducible a SQL. |
| 2 | `using Taller.Domain; // Modelo de persistencia interno.` | Modelo de persistencia interno. |
| 3 | `namespace Taller.Application.Clientes; // Datos públicos del caso de uso.` | Datos públicos del caso de uso. |
| 5 | `/// <summary>Respuesta explícita que evita exponer entidades de dominio.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 6 | `public sealed class ClienteDto` | Declara el tipo y sus dependencias o clase base. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `public Guid Id { get; init; } // Identificador público.` | Identificador público. |
| 9 | `public DateTimeOffset CreadoUtc { get; init; } // Registro UTC.` | Registro UTC. |
| 10 | `public string Nombre { get; init; } = string.Empty; // Nombre del cliente.` | Nombre del cliente. |
| 11 | `public string ApellidoPaterno { get; init; } = string.Empty; // Apellido paterno.` | Apellido paterno. |
| 12 | `public string? ApellidoMaterno { get; init; } // Apellido materno opcional.` | Apellido materno opcional. |
| 13 | `public string Telefono { get; init; } = string.Empty; // Teléfono fijo.` | Teléfono fijo. |
| 14 | `public string TelefonoCelular { get; init; } = string.Empty; // Teléfono celular.` | Teléfono celular. |
| 15 | `public string Email { get; init; } = string.Empty; // Correo del cliente.` | Correo del cliente. |
| 16 | `public string TelefonoContacto { get; init; } = string.Empty; // Teléfono del contacto.` | Teléfono del contacto. |
| 17 | `public string NombreCompletoContacto { get; init; } = string.Empty; // Nombre completo del contacto.` | Nombre completo del contacto. |
| 18 | `public string EmailContacto { get; init; } = string.Empty; // Correo del contacto.` | Correo del contacto. |
| 19 | `public string Calle { get; init; } = string.Empty; // Calle y número.` | Calle y número. |
| 20 | `public string Colonia { get; init; } = string.Empty; // Colonia sin duplicar el campo.` | Colonia sin duplicar el campo. |
| 21 | `public string Municipio { get; init; } = string.Empty; // Municipio de residencia.` | Municipio de residencia. |
| 22 | `public string Estado { get; init; } = string.Empty; // Estado de residencia.` | Estado de residencia. |
| 23 | `public string CodigoPostal { get; init; } = string.Empty; // Código postal mexicano de cinco dígitos.` | Código postal mexicano de cinco dígitos. |
| 25 | `public static readonly Expression<Func<Cliente, ClienteDto>> Projection = cliente => new ClienteDto // EF ejecuta esta proyección en SQL.` | EF ejecuta esta proyección en SQL. |
| 26 | `{` | Abre el bloque de definición o inicialización precedente. |
| 27 | `Id = cliente.Id, // Expone el identificador.` | Expone el identificador. |
| 28 | `CreadoUtc = cliente.CreadoUtc, // Expone la fecha de registro.` | Expone la fecha de registro. |
| 29 | `Nombre = cliente.Nombre, // Copia nombre del cliente.` | Copia nombre del cliente. |
| 30 | `ApellidoPaterno = cliente.ApellidoPaterno, // Copia apellido paterno.` | Copia apellido paterno. |
| 31 | `ApellidoMaterno = cliente.ApellidoMaterno, // Copia apellido materno opcional.` | Copia apellido materno opcional. |
| 32 | `Telefono = cliente.Telefono, // Copia teléfono fijo.` | Copia teléfono fijo. |
| 33 | `TelefonoCelular = cliente.TelefonoCelular, // Copia teléfono celular.` | Copia teléfono celular. |
| 34 | `Email = cliente.Email, // Copia correo del cliente.` | Copia correo del cliente. |
| 35 | `TelefonoContacto = cliente.TelefonoContacto, // Copia teléfono del contacto.` | Copia teléfono del contacto. |
| 36 | `NombreCompletoContacto = cliente.NombreCompletoContacto, // Copia nombre completo del contacto.` | Copia nombre completo del contacto. |
| 37 | `EmailContacto = cliente.EmailContacto, // Copia correo del contacto.` | Copia correo del contacto. |
| 38 | `Calle = cliente.Calle, // Copia calle y número.` | Copia calle y número. |
| 39 | `Colonia = cliente.Colonia, // Copia colonia sin duplicar el campo.` | Copia colonia sin duplicar el campo. |
| 40 | `Municipio = cliente.Municipio, // Copia municipio de residencia.` | Copia municipio de residencia. |
| 41 | `Estado = cliente.Estado, // Copia estado de residencia.` | Copia estado de residencia. |
| 42 | `CodigoPostal = cliente.CodigoPostal, // Copia código postal mexicano de cinco dígitos.` | Copia código postal mexicano de cinco dígitos. |
| 43 | `};` | Cierra el bloque, inicialización o llamada precedente. |
| 44 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Application\Clientes\ClienteService.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.ComponentModel.DataAnnotations; // Validación también fuera del controlador.` | Validación también fuera del controlador. |
| 2 | `using Taller.Domain; // Entidad del módulo.` | Entidad del módulo. |
| 3 | `namespace Taller.Application.Clientes; // Casos de uso independientes de HTTP.` | Casos de uso independientes de HTTP. |
| 5 | `/// <summary>Coordina validación, normalización y persistencia.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 6 | `public sealed class ClienteService(IClienteRepository repository)` | Declara el tipo y sus dependencias o clase base. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `public Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken) // Propaga cancelación.` | Propaga cancelación. |
| 9 | `{` | Abre el bloque de definición o inicialización precedente. |
| 10 | `Validator.ValidateObject(query, new ValidationContext(query), true); // Protege llamadas fuera de HTTP.` | Protege llamadas fuera de HTTP. |
| 11 | `return repository.ListAsync(query, cancellationToken); // Delega consulta al puerto.` | Delega consulta al puerto. |
| 12 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 14 | `public async Task<ClienteDto> CreateAsync(CrearClienteRequest request, CancellationToken cancellationToken) // Caso de uso de alta.` | Caso de uso de alta. |
| 15 | `{` | Abre el bloque de definición o inicialización precedente. |
| 16 | `Validator.ValidateObject(request, new ValidationContext(request), true); // Valida todos los atributos.` | Valida todos los atributos. |
| 17 | `var cliente = new Cliente // Asigna únicamente campos autorizados.` | Asigna únicamente campos autorizados. |
| 18 | `{` | Abre el bloque de definición o inicialización precedente. |
| 19 | `Nombre = request.Nombre.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 20 | `ApellidoPaterno = request.ApellidoPaterno.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 21 | `ApellidoMaterno = string.IsNullOrWhiteSpace(request.ApellidoMaterno) ? null : request.ApellidoMaterno.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 22 | `Telefono = request.Telefono.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 23 | `TelefonoCelular = request.TelefonoCelular.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 24 | `Email = request.Email.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 25 | `TelefonoContacto = request.TelefonoContacto.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 26 | `NombreCompletoContacto = request.NombreCompletoContacto.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 27 | `EmailContacto = request.EmailContacto.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 28 | `Calle = request.Calle.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 29 | `Colonia = request.Colonia.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 30 | `Municipio = request.Municipio.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 31 | `Estado = request.Estado.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 32 | `CodigoPostal = request.CodigoPostal.Trim(), // Normaliza bordes sin modificar el contenido.` | Normaliza bordes sin modificar el contenido. |
| 33 | `};` | Cierra el bloque, inicialización o llamada precedente. |
| 34 | `await repository.AddAsync(cliente, cancellationToken); // Confirma antes de responder éxito.` | Confirma antes de responder éxito. |
| 35 | `return Map(cliente); // Nunca devuelve la entidad EF.` | Nunca devuelve la entidad EF. |
| 36 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 38 | `private static readonly Func<Cliente, ClienteDto> Map = ClienteDto.Projection.Compile(); // Compila una vez el mapeo de escritura.` | Compila una vez el mapeo de escritura. |
| 39 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Application\Clientes\CrearClienteRequest.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.ComponentModel.DataAnnotations; // Reglas de entrada reutilizables.` | Reglas de entrada reutilizables. |
| 2 | `namespace Taller.Application.Clientes; // Contratos de aplicación.` | Contratos de aplicación. |
| 4 | `/// <summary>Datos aceptados para registrar un cliente; no permite asignar Id ni fecha.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 5 | `public sealed class CrearClienteRequest` | Declara el tipo y sus dependencias o clase base. |
| 6 | `{` | Abre el bloque de definición o inicialización precedente. |
| 7 | `[Required] // Nombre del cliente requerido; rechaza valores vacíos o espacios.` | Nombre del cliente requerido; rechaza valores vacíos o espacios. |
| 8 | `[StringLength(100)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 9 | `public string Nombre { get; init; } = string.Empty; // Nombre del cliente.` | Nombre del cliente. |
| 10 | `[Required] // Apellido paterno requerido; rechaza valores vacíos o espacios.` | Apellido paterno requerido; rechaza valores vacíos o espacios. |
| 11 | `[StringLength(100)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 12 | `public string ApellidoPaterno { get; init; } = string.Empty; // Apellido paterno.` | Apellido paterno. |
| 13 | `[StringLength(100)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 14 | `public string? ApellidoMaterno { get; init; } // Apellido materno opcional.` | Apellido materno opcional. |
| 15 | `[Required] // Teléfono fijo requerido; rechaza valores vacíos o espacios.` | Teléfono fijo requerido; rechaza valores vacíos o espacios. |
| 16 | `[StringLength(25)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 17 | `[RegularExpression(@"^\+?[0-9][0-9 ()-]{6,24}$")] // Admite prefijo internacional y formato legible.` | Admite prefijo internacional y formato legible. |
| 18 | `public string Telefono { get; init; } = string.Empty; // Teléfono fijo.` | Teléfono fijo. |
| 19 | `[Required] // Teléfono celular requerido; rechaza valores vacíos o espacios.` | Teléfono celular requerido; rechaza valores vacíos o espacios. |
| 20 | `[StringLength(25)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 21 | `[RegularExpression(@"^\+?[0-9][0-9 ()-]{6,24}$")] // Admite prefijo internacional y formato legible.` | Admite prefijo internacional y formato legible. |
| 22 | `public string TelefonoCelular { get; init; } = string.Empty; // Teléfono celular.` | Teléfono celular. |
| 23 | `[Required] // Correo del cliente requerido; rechaza valores vacíos o espacios.` | Correo del cliente requerido; rechaza valores vacíos o espacios. |
| 24 | `[StringLength(254)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 25 | `[EmailAddress] // Rechaza sintaxis de correo inválida.` | Rechaza sintaxis de correo inválida. |
| 26 | `public string Email { get; init; } = string.Empty; // Correo del cliente.` | Correo del cliente. |
| 27 | `[Required] // Teléfono del contacto requerido; rechaza valores vacíos o espacios.` | Teléfono del contacto requerido; rechaza valores vacíos o espacios. |
| 28 | `[StringLength(25)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 29 | `[RegularExpression(@"^\+?[0-9][0-9 ()-]{6,24}$")] // Admite prefijo internacional y formato legible.` | Admite prefijo internacional y formato legible. |
| 30 | `public string TelefonoContacto { get; init; } = string.Empty; // Teléfono del contacto.` | Teléfono del contacto. |
| 31 | `[Required] // Nombre completo del contacto requerido; rechaza valores vacíos o espacios.` | Nombre completo del contacto requerido; rechaza valores vacíos o espacios. |
| 32 | `[StringLength(200)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 33 | `public string NombreCompletoContacto { get; init; } = string.Empty; // Nombre completo del contacto.` | Nombre completo del contacto. |
| 34 | `[Required] // Correo del contacto requerido; rechaza valores vacíos o espacios.` | Correo del contacto requerido; rechaza valores vacíos o espacios. |
| 35 | `[StringLength(254)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 36 | `[EmailAddress] // Rechaza sintaxis de correo inválida.` | Rechaza sintaxis de correo inválida. |
| 37 | `public string EmailContacto { get; init; } = string.Empty; // Correo del contacto.` | Correo del contacto. |
| 38 | `[Required] // Calle y número requerido; rechaza valores vacíos o espacios.` | Calle y número requerido; rechaza valores vacíos o espacios. |
| 39 | `[StringLength(200)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 40 | `public string Calle { get; init; } = string.Empty; // Calle y número.` | Calle y número. |
| 41 | `[Required] // Colonia sin duplicar el campo requerido; rechaza valores vacíos o espacios.` | Colonia sin duplicar el campo requerido; rechaza valores vacíos o espacios. |
| 42 | `[StringLength(150)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 43 | `public string Colonia { get; init; } = string.Empty; // Colonia sin duplicar el campo.` | Colonia sin duplicar el campo. |
| 44 | `[Required] // Municipio de residencia requerido; rechaza valores vacíos o espacios.` | Municipio de residencia requerido; rechaza valores vacíos o espacios. |
| 45 | `[StringLength(150)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 46 | `public string Municipio { get; init; } = string.Empty; // Municipio de residencia.` | Municipio de residencia. |
| 47 | `[Required] // Estado de residencia requerido; rechaza valores vacíos o espacios.` | Estado de residencia requerido; rechaza valores vacíos o espacios. |
| 48 | `[StringLength(100)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 49 | `public string Estado { get; init; } = string.Empty; // Estado de residencia.` | Estado de residencia. |
| 50 | `[Required] // Código postal mexicano de cinco dígitos requerido; rechaza valores vacíos o espacios.` | Código postal mexicano de cinco dígitos requerido; rechaza valores vacíos o espacios. |
| 51 | `[StringLength(5)] // Longitud consistente con la columna SQL.` | Longitud consistente con la columna SQL. |
| 52 | `[RegularExpression(@"^[0-9]{5}$")] // Conserva ceros iniciales del código postal.` | Conserva ceros iniciales del código postal. |
| 53 | `public string CodigoPostal { get; init; } = string.Empty; // Código postal mexicano de cinco dígitos.` | Código postal mexicano de cinco dígitos. |
| 54 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Application\Clientes\IClienteRepository.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using Taller.Domain; // Entidad independiente de EF Core.` | Entidad independiente de EF Core. |
| 2 | `namespace Taller.Application.Clientes; // Puerto de persistencia del módulo.` | Puerto de persistencia del módulo. |
| 4 | `/// <summary>Operaciones específicas; no expone IQueryable ni detalles del proveedor.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 5 | `public interface IClienteRepository` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 6 | `{` | Abre el bloque de definición o inicialización precedente. |
| 7 | `Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken); // Consulta proyectada y acotada.` | Consulta proyectada y acotada. |
| 8 | `Task AddAsync(Cliente cliente, CancellationToken cancellationToken); // Guarda una unidad de trabajo atómica.` | Guarda una unidad de trabajo atómica. |
| 9 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Application\Clientes\Paginacion.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.ComponentModel.DataAnnotations; // Validación declarativa del contrato de consulta.` | Validación declarativa del contrato de consulta. |
| 2 | `namespace Taller.Application.Clientes; // Contratos del módulo de clientes.` | Contratos del módulo de clientes. |
| 4 | `/// <summary>Consulta acotada para evitar respuestas y desplazamientos ilimitados.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 5 | `public sealed class ClientesQuery` | Declara el tipo y sus dependencias o clase base. |
| 6 | `{` | Abre el bloque de definición o inicialización precedente. |
| 7 | `[Range(1, 10000)] // Impide páginas inválidas y offsets excesivos.` | Impide páginas inválidas y offsets excesivos. |
| 8 | `public int PageNumber { get; init; } = 1; // Primera página por defecto.` | Primera página por defecto. |
| 9 | `[Range(1, 100)] // Limita memoria, serialización y trabajo de la base.` | Limita memoria, serialización y trabajo de la base. |
| 10 | `public int PageSize { get; init; } = 20; // Tamaño conservador por defecto.` | Tamaño conservador por defecto. |
| 11 | `[Required, RegularExpression("^(asc&#124;desc)$")] // Lista permitida, sin SQL dinámico.` | Lista permitida, sin SQL dinámico. |
| 12 | `public string SortDirection { get; init; } = "asc"; // Orden ascendente por nombre.` | Orden ascendente por nombre. |
| 13 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 15 | `/// <summary>Datos y metadatos de una página; el total corresponde al momento del conteo.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 16 | `public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 17 | `{` | Abre el bloque de definición o inicialización precedente. |
| 18 | `public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); // Evita división entera.` | Evita división entera. |
| 19 | `public bool HasNextPage => PageNumber < TotalPages; // Permite navegar sin inferencias del consumidor.` | Permite navegar sin inferencias del consumidor. |
| 20 | `public bool HasPreviousPage => PageNumber > 1; // Señala si existe una página anterior.` | Señala si existe una página anterior. |
| 21 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Domain\Cliente.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `namespace Taller.Domain; // Modelo sin dependencias de infraestructura.` | Modelo sin dependencias de infraestructura. |
| 3 | `/// <summary>Cliente persistido; los datos se normalizan y validan en el caso de uso.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 4 | `public sealed class Cliente` | Declara el tipo y sus dependencias o clase base. |
| 5 | `{` | Abre el bloque de definición o inicialización precedente. |
| 6 | `public Guid Id { get; init; } = Guid.NewGuid(); // Identificador generado antes de persistir.` | Identificador generado antes de persistir. |
| 7 | `public DateTimeOffset CreadoUtc { get; init; } = DateTimeOffset.UtcNow; // Momento de registro en UTC.` | Momento de registro en UTC. |
| 8 | `public string Nombre { get; init; } = string.Empty; // Nombre del cliente.` | Nombre del cliente. |
| 9 | `public string ApellidoPaterno { get; init; } = string.Empty; // Apellido paterno.` | Apellido paterno. |
| 10 | `public string? ApellidoMaterno { get; init; } // Apellido materno opcional.` | Apellido materno opcional. |
| 11 | `public string Telefono { get; init; } = string.Empty; // Teléfono fijo.` | Teléfono fijo. |
| 12 | `public string TelefonoCelular { get; init; } = string.Empty; // Teléfono celular.` | Teléfono celular. |
| 13 | `public string Email { get; init; } = string.Empty; // Correo del cliente.` | Correo del cliente. |
| 14 | `public string TelefonoContacto { get; init; } = string.Empty; // Teléfono del contacto.` | Teléfono del contacto. |
| 15 | `public string NombreCompletoContacto { get; init; } = string.Empty; // Nombre completo del contacto.` | Nombre completo del contacto. |
| 16 | `public string EmailContacto { get; init; } = string.Empty; // Correo del contacto.` | Correo del contacto. |
| 17 | `public string Calle { get; init; } = string.Empty; // Calle y número.` | Calle y número. |
| 18 | `public string Colonia { get; init; } = string.Empty; // Colonia sin duplicar el campo.` | Colonia sin duplicar el campo. |
| 19 | `public string Municipio { get; init; } = string.Empty; // Municipio de residencia.` | Municipio de residencia. |
| 20 | `public string Estado { get; init; } = string.Empty; // Estado de residencia.` | Estado de residencia. |
| 21 | `public string CodigoPostal { get; init; } = string.Empty; // Código postal mexicano de cinco dígitos.` | Código postal mexicano de cinco dígitos. |
| 22 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Infrastructure\ClienteRepository.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using Microsoft.EntityFrameworkCore; // Operadores SQL asíncronos.` | Operadores SQL asíncronos. |
| 2 | `using Taller.Application.Clientes; // Puerto y DTOs.` | Puerto y DTOs. |
| 3 | `using Taller.Domain; // Entidad de escritura.` | Entidad de escritura. |
| 4 | `namespace Taller.Infrastructure; // Adaptador SQL Server.` | Adaptador SQL Server. |
| 5 | `/// <summary>Consultas acotadas en servidor y escritura transaccional.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 6 | `public sealed class ClienteRepository(TallerDbContext db) : IClienteRepository` | Declara el tipo y sus dependencias o clase base. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `public async Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 9 | `{` | Abre el bloque de definición o inicialización precedente. |
| 10 | `var clientes = db.Clientes.AsNoTracking(); // Evita seguimiento en lecturas.` | Evita seguimiento en lecturas. |
| 11 | `var total = await clientes.CountAsync(cancellationToken); // Cuenta sin materializar registros.` | Cuenta sin materializar registros. |
| 12 | `var ordered = query.SortDirection == "desc" // Lista permitida, sin SQL dinámico.` | Lista permitida, sin SQL dinámico. |
| 13 | `? clientes.OrderByDescending(x => x.Nombre).ThenByDescending(x => x.Id) // Desempate estable.` | Desempate estable. |
| 14 | `: clientes.OrderBy(x => x.Nombre).ThenBy(x => x.Id); // Usa el índice compuesto.` | Usa el índice compuesto. |
| 15 | `var items = await ordered.Skip((query.PageNumber - 1) * query.PageSize) // Offset acotado por validación.` | Offset acotado por validación. |
| 16 | `.Take(query.PageSize) // Limita filas transmitidas.` | Limita filas transmitidas. |
| 17 | `.Select(ClienteDto.Projection) // Proyecta el contrato en SQL.` | Proyecta el contrato en SQL. |
| 18 | `.ToListAsync(cancellationToken); // Libera el hilo durante I/O.` | Libera el hilo durante I/O. |
| 19 | `return new(items, query.PageNumber, query.PageSize, total); // Adjunta navegación.` | Adjunta navegación. |
| 20 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 21 | `public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 22 | `{` | Abre el bloque de definición o inicialización precedente. |
| 23 | `db.Clientes.Add(cliente); // Prepara inserción sin consultar SQL.` | Prepara inserción sin consultar SQL. |
| 24 | `await db.SaveChangesAsync(cancellationToken); // Confirma atómicamente sin reintentos ciegos.` | Confirma atómicamente sin reintentos ciegos. |
| 25 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 26 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Infrastructure\Migrations\20261007183209_InitialClientes.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System;` | Importa System para resolver los tipos utilizados. |
| 2 | `using Microsoft.EntityFrameworkCore.Migrations;` | Importa Microsoft.EntityFrameworkCore.Migrations para resolver los tipos utilizados. |
| 4 | `#nullable disable` | Configura anotaciones de nulabilidad del archivo generado por EF. |
| 6 | `namespace Taller.Infrastructure.Migrations` | Agrupa los tipos bajo Taller.Infrastructure.Migrations. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `/// <inheritdoc />` | Documentación XML del miembro o contrato que sigue. |
| 9 | `public partial class InitialClientes : Migration` | Declara el tipo y sus dependencias o clase base. |
| 10 | `{` | Abre el bloque de definición o inicialización precedente. |
| 11 | `/// <inheritdoc />` | Documentación XML del miembro o contrato que sigue. |
| 12 | `protected override void Up(MigrationBuilder migrationBuilder)` | Define las operaciones de aplicación de la migración. |
| 13 | `{` | Abre el bloque de definición o inicialización precedente. |
| 14 | `migrationBuilder.CreateTable(` | Crea la tabla del agregado durante la migración Up. |
| 15 | `name: "Clientes",` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 16 | `columns: table => new` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 17 | `{` | Abre el bloque de definición o inicialización precedente. |
| 18 | `Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 19 | `CreadoUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 20 | `Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 21 | `ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 22 | `ApellidoMaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 23 | `Telefono = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 24 | `TelefonoCelular = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 25 | `Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 26 | `TelefonoContacto = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 27 | `NombreCompletoContacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 28 | `EmailContacto = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 29 | `Calle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 30 | `Colonia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 31 | `Municipio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 32 | `Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 33 | `CodigoPostal = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)` | Define el tipo SQL, longitud y nulabilidad de esta columna en la migración. |
| 34 | `},` | Cierra el bloque, inicialización o llamada precedente. |
| 35 | `constraints: table =>` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 36 | `{` | Abre el bloque de definición o inicialización precedente. |
| 37 | `table.PrimaryKey("PK_Clientes", x => x.Id);` | Configura la clave primaria de la tabla Clientes. |
| 38 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 40 | `migrationBuilder.CreateIndex(` | Crea el índice compuesto que soporta el orden del listado. |
| 41 | `name: "IX_Clientes_Nombre_Id",` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 42 | `table: "Clientes",` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 43 | `columns: new[] { "Nombre", "Id" });` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 44 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 46 | `/// <inheritdoc />` | Documentación XML del miembro o contrato que sigue. |
| 47 | `protected override void Down(MigrationBuilder migrationBuilder)` | Define las operaciones de reversión de la migración. |
| 48 | `{` | Abre el bloque de definición o inicialización precedente. |
| 49 | `migrationBuilder.DropTable(` | Revierte la tabla en Down; elimina sus datos si se ejecuta rollback. |
| 50 | `name: "Clientes");` | Proporciona el nombre o definición del objeto SQL a la operación precedente. |
| 51 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 52 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 53 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Infrastructure\Migrations\20261007183209_InitialClientes.Designer.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `// <auto-generated />` | Comentario de documentación; no ejecuta lógica. |
| 2 | `using System;` | Importa System para resolver los tipos utilizados. |
| 3 | `using Microsoft.EntityFrameworkCore;` | Importa Microsoft.EntityFrameworkCore para resolver los tipos utilizados. |
| 4 | `using Microsoft.EntityFrameworkCore.Infrastructure;` | Importa Microsoft.EntityFrameworkCore.Infrastructure para resolver los tipos utilizados. |
| 5 | `using Microsoft.EntityFrameworkCore.Metadata;` | Importa Microsoft.EntityFrameworkCore.Metadata para resolver los tipos utilizados. |
| 6 | `using Microsoft.EntityFrameworkCore.Migrations;` | Importa Microsoft.EntityFrameworkCore.Migrations para resolver los tipos utilizados. |
| 7 | `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;` | Importa Microsoft.EntityFrameworkCore.Storage.ValueConversion para resolver los tipos utilizados. |
| 8 | `using Taller.Infrastructure;` | Importa Taller.Infrastructure para resolver los tipos utilizados. |
| 10 | `#nullable disable` | Configura anotaciones de nulabilidad del archivo generado por EF. |
| 12 | `namespace Taller.Infrastructure.Migrations` | Agrupa los tipos bajo Taller.Infrastructure.Migrations. |
| 13 | `{` | Abre el bloque de definición o inicialización precedente. |
| 14 | `[DbContext(typeof(TallerDbContext))]` | Asocia los metadatos generados con TallerDbContext. |
| 15 | `[Migration("20261007183209_InitialClientes")]` | Registra el identificador único de la migración en EF. |
| 16 | `partial class InitialClientes` | Declara el tipo y sus dependencias o clase base. |
| 17 | `{` | Abre el bloque de definición o inicialización precedente. |
| 18 | `/// <inheritdoc />` | Documentación XML del miembro o contrato que sigue. |
| 19 | `protected override void BuildTargetModel(ModelBuilder modelBuilder)` | Reconstruye los metadatos EF del esquema correspondiente a esta versión. |
| 20 | `{` | Abre el bloque de definición o inicialización precedente. |
| 21 | `#pragma warning disable 612, 618` | Ajusta advertencias para metadatos generados compatibles con EF. |
| 22 | `modelBuilder` | Inicia la configuración encadenada del modelo de EF. |
| 23 | `.HasAnnotation("ProductVersion", "9.0.20")` | Guarda metadatos del proveedor o versión para comparar migraciones futuras. |
| 24 | `.HasAnnotation("Relational:MaxIdentifierLength", 128);` | Guarda metadatos del proveedor o versión para comparar migraciones futuras. |
| 26 | `SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);` | Establece la convención del proveedor; el Id UUID usa ValueGeneratedNever en el modelo. |
| 28 | `modelBuilder.Entity("Taller.Domain.Cliente", b =>` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 29 | `{` | Abre el bloque de definición o inicialización precedente. |
| 30 | `b.Property<Guid>("Id")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 31 | `.HasColumnType("uniqueidentifier");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 33 | `b.Property<string>("ApellidoMaterno")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 34 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 35 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 37 | `b.Property<string>("ApellidoPaterno")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 38 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 39 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 40 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 42 | `b.Property<string>("Calle")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 43 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 44 | `.HasMaxLength(200)` | Fija la longitud máxima de la propiedad precedente. |
| 45 | `.HasColumnType("nvarchar(200)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 47 | `b.Property<string>("CodigoPostal")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 48 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 49 | `.HasMaxLength(5)` | Fija la longitud máxima de la propiedad precedente. |
| 50 | `.HasColumnType("nvarchar(5)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 52 | `b.Property<string>("Colonia")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 53 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 54 | `.HasMaxLength(150)` | Fija la longitud máxima de la propiedad precedente. |
| 55 | `.HasColumnType("nvarchar(150)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 57 | `b.Property<DateTimeOffset>("CreadoUtc")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 58 | `.HasColumnType("datetimeoffset");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 60 | `b.Property<string>("Email")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 61 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 62 | `.HasMaxLength(254)` | Fija la longitud máxima de la propiedad precedente. |
| 63 | `.HasColumnType("nvarchar(254)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 65 | `b.Property<string>("EmailContacto")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 66 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 67 | `.HasMaxLength(254)` | Fija la longitud máxima de la propiedad precedente. |
| 68 | `.HasColumnType("nvarchar(254)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 70 | `b.Property<string>("Estado")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 71 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 72 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 73 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 75 | `b.Property<string>("Municipio")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 76 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 77 | `.HasMaxLength(150)` | Fija la longitud máxima de la propiedad precedente. |
| 78 | `.HasColumnType("nvarchar(150)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 80 | `b.Property<string>("Nombre")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 81 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 82 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 83 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 85 | `b.Property<string>("NombreCompletoContacto")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 86 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 87 | `.HasMaxLength(200)` | Fija la longitud máxima de la propiedad precedente. |
| 88 | `.HasColumnType("nvarchar(200)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 90 | `b.Property<string>("Telefono")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 91 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 92 | `.HasMaxLength(25)` | Fija la longitud máxima de la propiedad precedente. |
| 93 | `.HasColumnType("nvarchar(25)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 95 | `b.Property<string>("TelefonoCelular")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 96 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 97 | `.HasMaxLength(25)` | Fija la longitud máxima de la propiedad precedente. |
| 98 | `.HasColumnType("nvarchar(25)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 100 | `b.Property<string>("TelefonoContacto")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 101 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 102 | `.HasMaxLength(25)` | Fija la longitud máxima de la propiedad precedente. |
| 103 | `.HasColumnType("nvarchar(25)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 105 | `b.HasKey("Id");` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 107 | `b.HasIndex("Nombre", "Id");` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 109 | `b.ToTable("Clientes", (string)null);` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 110 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 111 | `#pragma warning restore 612, 618` | Ajusta advertencias para metadatos generados compatibles con EF. |
| 112 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 113 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 114 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Infrastructure\Migrations\TallerDbContextModelSnapshot.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `// <auto-generated />` | Comentario de documentación; no ejecuta lógica. |
| 2 | `using System;` | Importa System para resolver los tipos utilizados. |
| 3 | `using Microsoft.EntityFrameworkCore;` | Importa Microsoft.EntityFrameworkCore para resolver los tipos utilizados. |
| 4 | `using Microsoft.EntityFrameworkCore.Infrastructure;` | Importa Microsoft.EntityFrameworkCore.Infrastructure para resolver los tipos utilizados. |
| 5 | `using Microsoft.EntityFrameworkCore.Metadata;` | Importa Microsoft.EntityFrameworkCore.Metadata para resolver los tipos utilizados. |
| 6 | `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;` | Importa Microsoft.EntityFrameworkCore.Storage.ValueConversion para resolver los tipos utilizados. |
| 7 | `using Taller.Infrastructure;` | Importa Taller.Infrastructure para resolver los tipos utilizados. |
| 9 | `#nullable disable` | Configura anotaciones de nulabilidad del archivo generado por EF. |
| 11 | `namespace Taller.Infrastructure.Migrations` | Agrupa los tipos bajo Taller.Infrastructure.Migrations. |
| 12 | `{` | Abre el bloque de definición o inicialización precedente. |
| 13 | `[DbContext(typeof(TallerDbContext))]` | Asocia los metadatos generados con TallerDbContext. |
| 14 | `partial class TallerDbContextModelSnapshot : ModelSnapshot` | Declara el tipo y sus dependencias o clase base. |
| 15 | `{` | Abre el bloque de definición o inicialización precedente. |
| 16 | `protected override void BuildModel(ModelBuilder modelBuilder)` | Reconstruye los metadatos EF del esquema correspondiente a esta versión. |
| 17 | `{` | Abre el bloque de definición o inicialización precedente. |
| 18 | `#pragma warning disable 612, 618` | Ajusta advertencias para metadatos generados compatibles con EF. |
| 19 | `modelBuilder` | Inicia la configuración encadenada del modelo de EF. |
| 20 | `.HasAnnotation("ProductVersion", "9.0.20")` | Guarda metadatos del proveedor o versión para comparar migraciones futuras. |
| 21 | `.HasAnnotation("Relational:MaxIdentifierLength", 128);` | Guarda metadatos del proveedor o versión para comparar migraciones futuras. |
| 23 | `SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);` | Establece la convención del proveedor; el Id UUID usa ValueGeneratedNever en el modelo. |
| 25 | `modelBuilder.Entity("Taller.Domain.Cliente", b =>` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 26 | `{` | Abre el bloque de definición o inicialización precedente. |
| 27 | `b.Property<Guid>("Id")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 28 | `.HasColumnType("uniqueidentifier");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 30 | `b.Property<string>("ApellidoMaterno")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 31 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 32 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 34 | `b.Property<string>("ApellidoPaterno")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 35 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 36 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 37 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 39 | `b.Property<string>("Calle")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 40 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 41 | `.HasMaxLength(200)` | Fija la longitud máxima de la propiedad precedente. |
| 42 | `.HasColumnType("nvarchar(200)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 44 | `b.Property<string>("CodigoPostal")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 45 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 46 | `.HasMaxLength(5)` | Fija la longitud máxima de la propiedad precedente. |
| 47 | `.HasColumnType("nvarchar(5)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 49 | `b.Property<string>("Colonia")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 50 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 51 | `.HasMaxLength(150)` | Fija la longitud máxima de la propiedad precedente. |
| 52 | `.HasColumnType("nvarchar(150)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 54 | `b.Property<DateTimeOffset>("CreadoUtc")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 55 | `.HasColumnType("datetimeoffset");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 57 | `b.Property<string>("Email")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 58 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 59 | `.HasMaxLength(254)` | Fija la longitud máxima de la propiedad precedente. |
| 60 | `.HasColumnType("nvarchar(254)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 62 | `b.Property<string>("EmailContacto")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 63 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 64 | `.HasMaxLength(254)` | Fija la longitud máxima de la propiedad precedente. |
| 65 | `.HasColumnType("nvarchar(254)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 67 | `b.Property<string>("Estado")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 68 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 69 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 70 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 72 | `b.Property<string>("Municipio")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 73 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 74 | `.HasMaxLength(150)` | Fija la longitud máxima de la propiedad precedente. |
| 75 | `.HasColumnType("nvarchar(150)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 77 | `b.Property<string>("Nombre")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 78 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 79 | `.HasMaxLength(100)` | Fija la longitud máxima de la propiedad precedente. |
| 80 | `.HasColumnType("nvarchar(100)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 82 | `b.Property<string>("NombreCompletoContacto")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 83 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 84 | `.HasMaxLength(200)` | Fija la longitud máxima de la propiedad precedente. |
| 85 | `.HasColumnType("nvarchar(200)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 87 | `b.Property<string>("Telefono")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 88 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 89 | `.HasMaxLength(25)` | Fija la longitud máxima de la propiedad precedente. |
| 90 | `.HasColumnType("nvarchar(25)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 92 | `b.Property<string>("TelefonoCelular")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 93 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 94 | `.HasMaxLength(25)` | Fija la longitud máxima de la propiedad precedente. |
| 95 | `.HasColumnType("nvarchar(25)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 97 | `b.Property<string>("TelefonoContacto")` | Declara los metadatos de la propiedad indicada para el modelo EF. |
| 98 | `.IsRequired()` | Declara que la columna precedente no admite NULL. |
| 99 | `.HasMaxLength(25)` | Fija la longitud máxima de la propiedad precedente. |
| 100 | `.HasColumnType("nvarchar(25)");` | Fija el tipo relacional SQL Server de la propiedad precedente. |
| 102 | `b.HasKey("Id");` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 104 | `b.HasIndex("Nombre", "Id");` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 106 | `b.ToTable("Clientes", (string)null);` | Registra la clave, índice, tabla o entidad nombrada dentro del modelo generado. |
| 107 | `});` | Cierra el bloque, inicialización o llamada precedente. |
| 108 | `#pragma warning restore 612, 618` | Ajusta advertencias para metadatos generados compatibles con EF. |
| 109 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 110 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 111 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Infrastructure\TallerDbContext.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using Microsoft.EntityFrameworkCore; // Mapeo relacional.` | Mapeo relacional. |
| 2 | `using Taller.Domain; // Entidad persistida.` | Entidad persistida. |
| 3 | `namespace Taller.Infrastructure; // Adaptadores externos.` | Adaptadores externos. |
| 5 | `/// <summary>Unidad de trabajo scoped; no se comparte entre solicitudes.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 6 | `public sealed class TallerDbContext(DbContextOptions<TallerDbContext> options) : DbContext(options)` | Declara el tipo y sus dependencias o clase base. |
| 7 | `{` | Abre el bloque de definición o inicialización precedente. |
| 8 | `public DbSet<Cliente> Clientes => Set<Cliente>(); // Acceso tipado al agregado.` | Acceso tipado al agregado. |
| 9 | `protected override void OnModelCreating(ModelBuilder modelBuilder) // Define esquema sin anotaciones en dominio.` | Define esquema sin anotaciones en dominio. |
| 10 | `{` | Abre el bloque de definición o inicialización precedente. |
| 11 | `var cliente = modelBuilder.Entity<Cliente>(); // Configura la única entidad inicial.` | Configura la única entidad inicial. |
| 12 | `cliente.ToTable("Clientes"); // Nombre estable de tabla.` | Nombre estable de tabla. |
| 13 | `cliente.HasKey(x => x.Id); // Clave primaria.` | Clave primaria. |
| 14 | `cliente.Property(x => x.Id).ValueGeneratedNever(); // El dominio genera el UUID.` | El dominio genera el UUID. |
| 15 | `cliente.HasIndex(x => new { x.Nombre, x.Id }); // Soporta orden determinista y paginado.` | Soporta orden determinista y paginado. |
| 16 | `cliente.Property(x => x.Nombre).HasMaxLength(100).IsRequired(); // Limita nombre del cliente.` | Limita nombre del cliente. |
| 17 | `cliente.Property(x => x.ApellidoPaterno).HasMaxLength(100).IsRequired(); // Limita apellido paterno.` | Limita apellido paterno. |
| 18 | `cliente.Property(x => x.ApellidoMaterno).HasMaxLength(100); // Limita apellido materno opcional.` | Limita apellido materno opcional. |
| 19 | `cliente.Property(x => x.Telefono).HasMaxLength(25).IsRequired(); // Limita teléfono fijo.` | Limita teléfono fijo. |
| 20 | `cliente.Property(x => x.TelefonoCelular).HasMaxLength(25).IsRequired(); // Limita teléfono celular.` | Limita teléfono celular. |
| 21 | `cliente.Property(x => x.Email).HasMaxLength(254).IsRequired(); // Limita correo del cliente.` | Limita correo del cliente. |
| 22 | `cliente.Property(x => x.TelefonoContacto).HasMaxLength(25).IsRequired(); // Limita teléfono del contacto.` | Limita teléfono del contacto. |
| 23 | `cliente.Property(x => x.NombreCompletoContacto).HasMaxLength(200).IsRequired(); // Limita nombre completo del contacto.` | Limita nombre completo del contacto. |
| 24 | `cliente.Property(x => x.EmailContacto).HasMaxLength(254).IsRequired(); // Limita correo del contacto.` | Limita correo del contacto. |
| 25 | `cliente.Property(x => x.Calle).HasMaxLength(200).IsRequired(); // Limita calle y número.` | Limita calle y número. |
| 26 | `cliente.Property(x => x.Colonia).HasMaxLength(150).IsRequired(); // Limita colonia sin duplicar el campo.` | Limita colonia sin duplicar el campo. |
| 27 | `cliente.Property(x => x.Municipio).HasMaxLength(150).IsRequired(); // Limita municipio de residencia.` | Limita municipio de residencia. |
| 28 | `cliente.Property(x => x.Estado).HasMaxLength(100).IsRequired(); // Limita estado de residencia.` | Limita estado de residencia. |
| 29 | `cliente.Property(x => x.CodigoPostal).HasMaxLength(5).IsRequired(); // Limita código postal mexicano de cinco dígitos.` | Limita código postal mexicano de cinco dígitos. |
| 30 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 31 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Infrastructure\TallerDbContextFactory.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using Microsoft.EntityFrameworkCore; // Configura el proveedor SQL Server.` | Configura el proveedor SQL Server. |
| 2 | `using Microsoft.EntityFrameworkCore.Design; // Activación exclusiva de herramientas EF.` | Activación exclusiva de herramientas EF. |
| 3 | `namespace Taller.Infrastructure; // Infraestructura de persistencia.` | Infraestructura de persistencia. |
| 4 | `/// <summary>Permite migraciones sin iniciar HTTP ni configurar el proveedor de identidad.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 5 | `public sealed class TallerDbContextFactory : IDesignTimeDbContextFactory<TallerDbContext>` | Declara el tipo y sus dependencias o clase base. |
| 6 | `{` | Abre el bloque de definición o inicialización precedente. |
| 7 | `public TallerDbContext CreateDbContext(string[] args)` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 8 | `{` | Abre el bloque de definición o inicialización precedente. |
| 9 | `var connection = Environment.GetEnvironmentVariable("ConnectionStrings__Taller") // Usa la base explícita del despliegue.` | Usa la base explícita del despliegue. |
| 10 | `?? @"Server=(localdb)\MSSQLLocalDB;Database=TallerMecanicoDev;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"; // Solo fallback local de herramientas.` | Solo fallback local de herramientas. |
| 11 | `return new(new DbContextOptionsBuilder<TallerDbContext>().UseSqlServer(connection).Options); // No conecta hasta ejecutar una operación.` | No conecta hasta ejecutar una operación. |
| 12 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 13 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Tests\ClientesFactory.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.IdentityModel.Tokens.Jwt; // Generación exclusiva de tokens de prueba.` | Generación exclusiva de tokens de prueba. |
| 2 | `using System.Security.Claims; // Permisos de prueba.` | Permisos de prueba. |
| 3 | `using System.Security.Cryptography; // Clave efímera, nunca versionada.` | Clave efímera, nunca versionada. |
| 4 | `using Microsoft.AspNetCore.Authentication.JwtBearer; // Mantiene autenticación real.` | Mantiene autenticación real. |
| 5 | `using Microsoft.AspNetCore.Hosting; // Personaliza el host de pruebas.` | Personaliza el host de pruebas. |
| 6 | `using Microsoft.AspNetCore.Mvc.Testing; // Servidor HTTP en memoria.` | Servidor HTTP en memoria. |
| 7 | `using Microsoft.EntityFrameworkCore; // Migraciones reales.` | Migraciones reales. |
| 8 | `using Microsoft.Extensions.DependencyInjection; // Resolución scoped.` | Resolución scoped. |
| 9 | `using Microsoft.IdentityModel.Protocols; // Descubrimiento estático aislado.` | Descubrimiento estático aislado. |
| 10 | `using Microsoft.IdentityModel.Protocols.OpenIdConnect; // Configuración de emisor de prueba.` | Configuración de emisor de prueba. |
| 11 | `using Microsoft.IdentityModel.Tokens; // Firma y validación criptográfica.` | Firma y validación criptográfica. |
| 12 | `using Taller.Infrastructure; // DbContext productivo.` | DbContext productivo. |
| 13 | `using Xunit; // Ciclo de vida asíncrono.` | Ciclo de vida asíncrono. |
| 14 | `namespace Taller.Tests; // Pruebas aisladas del entorno productivo.` | Pruebas aisladas del entorno productivo. |
| 15 | `/// <summary>Usa SQL Server real con una base temporal única y JWT firmado con RSA efímero.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 16 | `public sealed class ClientesFactory : WebApplicationFactory<Program>, IAsyncLifetime` | Declara el tipo y sus dependencias o clase base. |
| 17 | `{` | Abre el bloque de definición o inicialización precedente. |
| 18 | `private readonly RSA rsa = RSA.Create(2048); // Clave efímera por ejecución.` | Clave efímera por ejecución. |
| 19 | `private readonly string database = "TallerTests_" + Guid.NewGuid().ToString("N"); // Evita tocar bases existentes.` | Evita tocar bases existentes. |
| 20 | `public HttpClient Client { get; private set; } = null!; // Se inicializa después del host.` | Se inicializa después del host. |
| 21 | `protected override void ConfigureWebHost(IWebHostBuilder builder)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 22 | `{` | Abre el bloque de definición o inicialización precedente. |
| 23 | `builder.UseEnvironment("Development"); // Habilita OpenAPI para verificar contrato.` | Habilita OpenAPI para verificar contrato. |
| 24 | `builder.UseSetting("ConnectionStrings:Taller", $@"Server=(localdb)\MSSQLLocalDB;Database={database};Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"); // Solo base temporal.` | Solo base temporal. |
| 25 | `builder.UseSetting("Authentication:Authority", "https://test-issuer.invalid"); // Emisor aislado.` | Emisor aislado. |
| 26 | `builder.UseSetting("Authentication:Audience", "taller-tests"); // Audiencia aislada.` | Audiencia aislada. |
| 27 | `builder.ConfigureServices(services => services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options => // Cambia claves, no omite autenticación.` | Cambia claves, no omite autenticación. |
| 28 | `{` | Abre el bloque de definición o inicialización precedente. |
| 29 | `var configuration = new OpenIdConnectConfiguration { Issuer = "https://test-issuer.invalid" }; // Sin llamadas externas.` | Sin llamadas externas. |
| 30 | `configuration.SigningKeys.Add(new RsaSecurityKey(rsa) { KeyId = "test-key" }); // Clave pública verificadora.` | Clave pública verificadora. |
| 31 | `options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration); // Descubrimiento determinista.` | Descubrimiento determinista. |
| 32 | `options.TokenValidationParameters.ValidAudience = "taller-tests"; // Rechaza otras audiencias.` | Rechaza otras audiencias. |
| 33 | `}));` | Cierra el bloque, inicialización o llamada precedente. |
| 34 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 35 | `public string Token(bool permission = true, bool expired = false, string audience = "taller-tests", bool badSignature = false)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 36 | `{` | Abre el bloque de definición o inicialización precedente. |
| 37 | `using var other = RSA.Create(2048); // Clave alternativa para verificar rechazo de firmas.` | Clave alternativa para verificar rechazo de firmas. |
| 38 | `var claims = permission ? new[] { new Claim("sub", "tester"), new Claim("permission", "clientes.manage") } : new[] { new Claim("sub", "tester") }; // Permisos controlados.` | Permisos controlados. |
| 39 | `var token = new JwtSecurityToken("https://test-issuer.invalid", audience, claims, DateTime.UtcNow.AddHours(-2),` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 40 | `expired ? DateTime.UtcNow.AddHours(-1) : DateTime.UtcNow.AddMinutes(5), // Prueba expiración real.` | Prueba expiración real. |
| 41 | `new SigningCredentials(new RsaSecurityKey(badSignature ? other : rsa) { KeyId = "test-key" }, SecurityAlgorithms.RsaSha256)); // Firma asimétrica.` | Firma asimétrica. |
| 42 | `return new JwtSecurityTokenHandler().WriteToken(token); // Token wire-format.` | Token wire-format. |
| 43 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 44 | `public async Task InitializeAsync()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 45 | `{` | Abre el bloque de definición o inicialización precedente. |
| 46 | `Client = CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false }); // Simula HTTPS.` | Simula HTTPS. |
| 47 | `using var scope = Services.CreateScope(); // DbContext independiente.` | DbContext independiente. |
| 48 | `await scope.ServiceProvider.GetRequiredService<TallerDbContext>().Database.MigrateAsync(); // Aplica migración sobre SQL Server real.` | Aplica migración sobre SQL Server real. |
| 49 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 50 | `async Task IAsyncLifetime.DisposeAsync()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 51 | `{` | Abre el bloque de definición o inicialización precedente. |
| 52 | `using var scope = Services.CreateScope(); // Limpieza de la base propiedad del fixture.` | Limpieza de la base propiedad del fixture. |
| 53 | `var db = scope.ServiceProvider.GetRequiredService<TallerDbContext>(); // Proveedor real.` | Proveedor real. |
| 54 | `if (db.Database.GetDbConnection().Database != database &#124;&#124; !database.StartsWith("TallerTests_")) throw new InvalidOperationException("Base no autorizada para limpieza."); // Guardia contra borrado accidental.` | Guardia contra borrado accidental. |
| 55 | `await db.Database.EnsureDeletedAsync(); // Elimina únicamente la base temporal única.` | Elimina únicamente la base temporal única. |
| 56 | `Client.Dispose(); // Libera conexiones HTTP.` | Libera conexiones HTTP. |
| 57 | `await DisposeAsync(); // Cierra el servidor.` | Cierra el servidor. |
| 58 | `rsa.Dispose(); // Destruye la clave efímera.` | Destruye la clave efímera. |
| 59 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 60 | `}` | Cierra el bloque, inicialización o llamada precedente. |

## Taller.Tests\ClientesTests.cs

| Línea | Código | Explicación |
|---:|---|---|
| 1 | `using System.Net; // Estados esperados.` | Estados esperados. |
| 2 | `using System.Net.Http.Headers; // Cabecera Bearer.` | Cabecera Bearer. |
| 3 | `using System.Net.Http.Json; // JSON HTTP.` | JSON HTTP. |
| 4 | `using System.Text; // Cuerpos inválidos.` | Cuerpos inválidos. |
| 5 | `using System.Text.Json; // Inspección de contratos.` | Inspección de contratos. |
| 6 | `using Xunit; // Aserciones y casos de prueba.` | Aserciones y casos de prueba. |
| 7 | `using Microsoft.AspNetCore.TestHost; // Configuración aislada para fallos deliberados.` | Configuración aislada para fallos deliberados. |
| 8 | `using Microsoft.Extensions.DependencyInjection; // Sustitución controlada de un puerto en prueba negativa.` | Sustitución controlada de un puerto en prueba negativa. |
| 9 | `using Taller.Application.Clientes; // Contratos del adaptador que falla.` | Contratos del adaptador que falla. |
| 10 | `using Taller.Domain; // Tipo del puerto de escritura.` | Tipo del puerto de escritura. |
| 11 | `namespace Taller.Tests; // Integración API y SQL Server.` | Integración API y SQL Server. |
| 12 | `/// <summary>Verifica comportamiento observable sin reemplazar repositorio, EF ni autenticación.</summary>` | Documentación XML del miembro o contrato que sigue. |
| 13 | `public sealed class ClientesTests(ClientesFactory factory) : IClassFixture<ClientesFactory>` | Declara el tipo y sus dependencias o clase base. |
| 14 | `{` | Abre el bloque de definición o inicialización precedente. |
| 15 | `private HttpRequestMessage Request(HttpMethod method, string path, string? token = null, object? body = null)` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 16 | `{` | Abre el bloque de definición o inicialización precedente. |
| 17 | `var request = new HttpRequestMessage(method, path); // Crea solicitud aislada.` | Crea solicitud aislada. |
| 18 | `if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // No comparte cabeceras mutables.` | No comparte cabeceras mutables. |
| 19 | `if (body is not null) request.Content = JsonContent.Create(body); // Usa JSON real.` | Usa JSON real. |
| 20 | `return request; // El llamador dispone la solicitud.` | El llamador dispone la solicitud. |
| 21 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 22 | `private static object Body(string nombre = "Ana", string email = "ana@example.test") => new // Datos ficticios sin PII real.` | Datos ficticios sin PII real. |
| 23 | `{` | Abre el bloque de definición o inicialización precedente. |
| 24 | `data = new { nombre, apellidoPaterno = "Perez", apellidoMaterno = "Lopez", telefono = "5551234567", telefonoCelular = "+525551234568", email,` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 25 | `telefonoContacto = "5551234569", nombreCompletoContacto = "Contacto Prueba", emailContacto = "contacto@example.test", calle = "Calle 1", colonia = "Centro", municipio = "Puebla", estado = "Puebla", codigoPostal = "01234" } // Contrato completo.` | Contrato completo. |
| 26 | `};` | Cierra el bloque, inicialización o llamada precedente. |
| 27 | `[Theory] // Asegura protección en ambos alias y verbos.` | Asegura protección en ambos alias y verbos. |
| 28 | `[InlineData("GET", "/api/clientes")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 29 | `[InlineData("POST", "/api/clientes")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 30 | `[InlineData("GET", "/api/v1/clientes")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 31 | `[InlineData("POST", "/api/v1/clientes")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 32 | `public async Task SinTokenDevuelve401(string method, string path)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 33 | `{` | Abre el bloque de definición o inicialización precedente. |
| 34 | `using var request = Request(new HttpMethod(method), path, body: method == "POST" ? Body() : null); // Sin credenciales.` | Sin credenciales. |
| 35 | `using var response = await factory.Client.SendAsync(request); // Ejecuta pipeline.` | Ejecuta pipeline. |
| 36 | `Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // No accede al caso de uso.` | No accede al caso de uso. |
| 37 | `Assert.Equal(401, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Wrapper uniforme.` | Wrapper uniforme. |
| 38 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 39 | `[Fact] // Token válido sin autorización.` | Token válido sin autorización. |
| 40 | `public async Task SinPermisoDevuelve403()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 41 | `{` | Abre el bloque de definición o inicialización precedente. |
| 42 | `using var request = Request(HttpMethod.Get, "/api/clientes", factory.Token(permission: false)); // Identidad sin permiso.` | Identidad sin permiso. |
| 43 | `using var response = await factory.Client.SendAsync(request); // Autenticación real.` | Autenticación real. |
| 44 | `Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode); // Política efectiva.` | Política efectiva. |
| 45 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 46 | `[Theory] // Rechaza tokens inválidos criptográfica o semánticamente.` | Rechaza tokens inválidos criptográfica o semánticamente. |
| 47 | `[InlineData(true, "taller-tests", false)]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 48 | `[InlineData(false, "otra-api", false)]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 49 | `[InlineData(false, "taller-tests", true)]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 50 | `public async Task TokenInvalidoDevuelve401(bool expired, string audience, bool badSignature)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 51 | `{` | Abre el bloque de definición o inicialización precedente. |
| 52 | `using var request = Request(HttpMethod.Get, "/api/clientes", factory.Token(expired: expired, audience: audience, badSignature: badSignature)); // Construye caso negativo.` | Construye caso negativo. |
| 53 | `using var response = await factory.Client.SendAsync(request); // Verifica el validador productivo.` | Verifica el validador productivo. |
| 54 | `Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Ningún token inválido entra.` | Ningún token inválido entra. |
| 55 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 56 | `[Theory] // Límites y allowlist de orden.` | Límites y allowlist de orden. |
| 57 | `[InlineData("pageNumber=0")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 58 | `[InlineData("pageNumber=10001")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 59 | `[InlineData("pageSize=101")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 60 | `[InlineData("pageSize=-1")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 61 | `[InlineData("sortDirection=drop")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 62 | `public async Task QueryInvalidaDevuelve400(string query)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 63 | `{` | Abre el bloque de definición o inicialización precedente. |
| 64 | `using var request = Request(HttpMethod.Get, "/api/clientes?" + query, factory.Token()); // Query inválida.` | Query inválida. |
| 65 | `using var response = await factory.Client.SendAsync(request); // Model binding real.` | Model binding real. |
| 66 | `Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Rechaza antes de consultar.` | Rechaza antes de consultar. |
| 67 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 68 | `[Fact] // Valida objetos anidados.` | Valida objetos anidados. |
| 69 | `public async Task EmailInvalidoDevuelve400()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 70 | `{` | Abre el bloque de definición o inicialización precedente. |
| 71 | `using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token(), Body(email: "invalido")); // Formato incorrecto.` | Formato incorrecto. |
| 72 | `using var response = await factory.Client.SendAsync(request); // Validación del DTO interno.` | Validación del DTO interno. |
| 73 | `Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // No persiste.` | No persiste. |
| 74 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 75 | `[Theory] // Cuerpos ausentes, vacíos y malformados.` | Cuerpos ausentes, vacíos y malformados. |
| 76 | `[InlineData("{}")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 77 | `[InlineData("{\"data\":null}")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 78 | `[InlineData("{\"data\":{}}")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 79 | `[InlineData("{no-json")]` | Continúa la expresión o declaración precedente; su comportamiento se documenta en el bloque y comentarios asociados. |
| 80 | `public async Task CuerpoInvalidoDevuelve400(string body)` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 81 | `{` | Abre el bloque de definición o inicialización precedente. |
| 82 | `using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token()); // Token autorizado.` | Token autorizado. |
| 83 | `request.Content = new StringContent(body, Encoding.UTF8, "application/json"); // Conserva cuerpo inválido.` | Conserva cuerpo inválido. |
| 84 | `using var response = await factory.Client.SendAsync(request); // Deserialización real.` | Deserialización real. |
| 85 | `Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // No produce un 500.` | No produce un 500. |
| 86 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 87 | `[Fact] // Flujo funcional persistido y ordenado.` | Flujo funcional persistido y ordenado. |
| 88 | `public async Task RegistroConsultaOrdenYPaginacion()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 89 | `{` | Abre el bloque de definición o inicialización precedente. |
| 90 | `foreach (var nombre in new[] { "Zoe", "Ana", "Ana", "Luis" }) // Incluye empate por nombre.` | Incluye empate por nombre. |
| 91 | `{` | Abre el bloque de definición o inicialización precedente. |
| 92 | `using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token(), Body(nombre)); // Alta real.` | Alta real. |
| 93 | `using var response = await factory.Client.SendAsync(request); // Guarda en SQL Server.` | Guarda en SQL Server. |
| 94 | `Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Confirma creación.` | Confirma creación. |
| 95 | `var json = await response.Content.ReadFromJsonAsync<JsonElement>(); // Inspecciona DTO.` | Inspecciona DTO. |
| 96 | `Assert.Equal(nombre, json.GetProperty("data").GetProperty("nombre").GetString()); // Mantiene datos.` | Mantiene datos. |
| 97 | `Assert.Equal("01234", json.GetProperty("data").GetProperty("codigoPostal").GetString()); // Preserva cero inicial.` | Preserva cero inicial. |
| 98 | `Assert.True(response.Headers.Contains("X-Correlation-ID")); // Trazabilidad.` | Trazabilidad. |
| 99 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 100 | `var ids = new List<string>(); // Acumula páginas para detectar solapamientos.` | Acumula páginas para detectar solapamientos. |
| 101 | `foreach (var page in new[] { 1, 2 }) // Recorre dos páginas consecutivas.` | Recorre dos páginas consecutivas. |
| 102 | `{` | Abre el bloque de definición o inicialización precedente. |
| 103 | `using var request = Request(HttpMethod.Get, $"/api/v1/clientes?pageNumber={page}&pageSize=2&sortDirection=asc", factory.Token()); // Alias versionado.` | Alias versionado. |
| 104 | `using var response = await factory.Client.SendAsync(request); // Consulta SQL paginada.` | Consulta SQL paginada. |
| 105 | `Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Éxito.` | Éxito. |
| 106 | `var data = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data"); // Sobre paginado.` | Sobre paginado. |
| 107 | `Assert.Equal(4, data.GetProperty("totalCount").GetInt32()); // Conteo persistido.` | Conteo persistido. |
| 108 | `Assert.Equal(2, data.GetProperty("totalPages").GetInt32()); // Metadatos correctos.` | Metadatos correctos. |
| 109 | `ids.AddRange(data.GetProperty("items").EnumerateArray().Select(x => x.GetProperty("id").GetString()!)); // Identificadores por página.` | Identificadores por página. |
| 110 | `Assert.Equal(page == 1 ? "Ana" : "Luis", data.GetProperty("items")[0].GetProperty("nombre").GetString()); // Orden esperado.` | Orden esperado. |
| 111 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 112 | `Assert.Equal(4, ids.Distinct().Count()); // Sin duplicados entre páginas estables.` | Sin duplicados entre páginas estables. |
| 113 | `using var descending = Request(HttpMethod.Get, "/api/clientes?pageSize=4&sortDirection=desc", factory.Token()); // Invierte todo el orden.` | Invierte todo el orden. |
| 114 | `using var descendingResponse = await factory.Client.SendAsync(descending); // Consulta inversa.` | Consulta inversa. |
| 115 | `var reverse = (await descendingResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data").GetProperty("items"); // Resultado descendente.` | Resultado descendente. |
| 116 | `Assert.Equal(ids.AsEnumerable().Reverse(), reverse.EnumerateArray().Select(x => x.GetProperty("id").GetString()!)); // Comprueba desempate inverso.` | Comprueba desempate inverso. |
| 117 | `using var beyond = Request(HttpMethod.Get, "/api/clientes?pageNumber=100", factory.Token()); // Página sin filas.` | Página sin filas. |
| 118 | `using var beyondResponse = await factory.Client.SendAsync(beyond); // Debe ser éxito vacío.` | Debe ser éxito vacío. |
| 119 | `Assert.Empty((await beyondResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data").GetProperty("items").EnumerateArray()); // Nunca 404 por página vacía.` | Nunca 404 por página vacía. |
| 120 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 121 | `[Fact] // Verifica exposición y documentación.` | Verifica exposición y documentación. |
| 122 | `public async Task OpenApiDocumentaLosDosAlias()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 123 | `{` | Abre el bloque de definición o inicialización precedente. |
| 124 | `var document = await factory.Client.GetFromJsonAsync<JsonElement>("/openapi/v1.json"); // Documento generado.` | Documento generado. |
| 125 | `foreach (var path in new[] { "/api/clientes", "/api/v1/clientes" }) // Dos rutas compatibles.` | Dos rutas compatibles. |
| 126 | `{` | Abre el bloque de definición o inicialización precedente. |
| 127 | `var item = document.GetProperty("paths").GetProperty(path); // Operaciones publicadas.` | Operaciones publicadas. |
| 128 | `Assert.True(item.TryGetProperty("get", out _)); // Consulta documentada.` | Consulta documentada. |
| 129 | `Assert.True(item.TryGetProperty("post", out _)); // Alta documentada.` | Alta documentada. |
| 130 | `Assert.True(item.GetProperty("get").TryGetProperty("security", out _)); // Bearer documentado.` | Bearer documentado. |
| 131 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 132 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 133 | `[Fact] // Un formato distinto de JSON conserva el sobre de error.` | Un formato distinto de JSON conserva el sobre de error. |
| 134 | `public async Task FormatoNoSoportadoDevuelve415()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 135 | `{` | Abre el bloque de definición o inicialización precedente. |
| 136 | `using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token()); // Usuario autorizado.` | Usuario autorizado. |
| 137 | `request.Content = new StringContent("texto", Encoding.UTF8, "text/plain"); // Formato no soportado.` | Formato no soportado. |
| 138 | `using var response = await factory.Client.SendAsync(request); // Ejecuta selección del formatter.` | Ejecuta selección del formatter. |
| 139 | `Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode); // HTTP 415.` | HTTP 415. |
| 140 | `Assert.Equal(415, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Sobre consistente.` | Sobre consistente. |
| 141 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 142 | `[Fact] // Prueba manejo de fallos sin alterar la base real del fixture.` | Prueba manejo de fallos sin alterar la base real del fixture. |
| 143 | `public async Task FalloInternoNoExponeDetalles()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 144 | `{` | Abre el bloque de definición o inicialización precedente. |
| 145 | `await using var failing = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddScoped<IClienteRepository, FailingRepository>())); // Puerto fallido solo en este host.` | Puerto fallido solo en este host. |
| 146 | `using var client = failing.CreateClient(new() { BaseAddress = new Uri("https://localhost") }); // Host independiente.` | Host independiente. |
| 147 | `using var request = Request(HttpMethod.Get, "/api/clientes", factory.Token()); // Token válido.` | Token válido. |
| 148 | `using var response = await client.SendAsync(request); // Provoca fallo tras autorización.` | Provoca fallo tras autorización. |
| 149 | `Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode); // HTTP 500.` | HTTP 500. |
| 150 | `Assert.DoesNotContain("dato-secreto", await response.Content.ReadAsStringAsync()); // Sanitización observable.` | Sanitización observable. |
| 151 | `Assert.Equal(500, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Wrapper correcto.` | Wrapper correcto. |
| 152 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 153 | `[Fact] // Límite activo incluso para solicitudes sin token.` | Límite activo incluso para solicitudes sin token. |
| 154 | `public async Task SaturacionDevuelve429()` | Declara la operación, sus parámetros y tipo de resultado; el cuerpo documenta cada paso. |
| 155 | `{` | Abre el bloque de definición o inicialización precedente. |
| 156 | `await using var isolated = factory.WithWebHostBuilder(_ => { }); // Presupuesto separado del resto de pruebas.` | Presupuesto separado del resto de pruebas. |
| 157 | `using var client = isolated.CreateClient(new() { BaseAddress = new Uri("https://localhost") }); // Solicitudes HTTPS.` | Solicitudes HTTPS. |
| 158 | `for (var index = 0; index <= 600; index++) // Supera la ventana sin tocar SQL.` | Supera la ventana sin tocar SQL. |
| 159 | `{` | Abre el bloque de definición o inicialización precedente. |
| 160 | `using var response = await client.GetAsync("/api/clientes"); // Solicitud anónima contabilizada.` | Solicitud anónima contabilizada. |
| 161 | `if (index == 600) // Primera solicitud fuera del presupuesto.` | Primera solicitud fuera del presupuesto. |
| 162 | `{` | Abre el bloque de definición o inicialización precedente. |
| 163 | `Assert.Equal((HttpStatusCode)429, response.StatusCode); // Protección efectiva.` | Protección efectiva. |
| 164 | `Assert.Equal(429, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Sobre uniforme.` | Sobre uniforme. |
| 165 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 166 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 167 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 168 | `private sealed class FailingRepository : IClienteRepository // Adaptador negativo exclusivo de pruebas.` | Adaptador negativo exclusivo de pruebas. |
| 169 | `{` | Abre el bloque de definición o inicialización precedente. |
| 170 | `public Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken) => throw new InvalidOperationException("dato-secreto"); // Simula detalle sensible.` | Simula detalle sensible. |
| 171 | `public Task AddAsync(Cliente cliente, CancellationToken cancellationToken) => throw new InvalidOperationException("dato-secreto"); // Nunca ejecuta SQL.` | Nunca ejecuta SQL. |
| 172 | `}` | Cierra el bloque, inicialización o llamada precedente. |
| 173 | `}` | Cierra el bloque, inicialización o llamada precedente. |
