using System.Diagnostics; // Trazas distribuidas.
using System.Net; // Estados HTTP.
using System.Threading.RateLimiting; // Protección de recursos.
using API.Principal.Contracts; // Wrappers.
using API.Principal.Middleware; // Errores globales.
using Microsoft.AspNetCore.Authentication.JwtBearer; // Autenticación JWT.
using Microsoft.AspNetCore.Mvc; // Validación MVC.
using Microsoft.EntityFrameworkCore; // Persistencia EF.
using Microsoft.OpenApi.Models; // Documenta autenticación Bearer y operaciones únicas.
using Taller.Application.Clientes; // Casos de uso.
using Taller.Infrastructure; // Adaptadores.

var builder = WebApplication.CreateBuilder(args); // Carga configuración externa.
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 32 * 1024); // Limita tamaño del cuerpo.
var connection = builder.Configuration.GetConnectionString("Taller") // Cadena externa, sin secretos versionados.
    ?? throw new InvalidOperationException("Configura ConnectionStrings:Taller."); // Falla temprano.
var authority = builder.Configuration["Authentication:Authority"]; // Emisor OIDC.
var audience = builder.Configuration["Authentication:Audience"]; // Destinatario esperado.
if (!Uri.TryCreate(authority, UriKind.Absolute, out var issuer) || issuer.Scheme != "https" || string.IsNullOrWhiteSpace(audience)) // Impide configuración insegura.
    throw new InvalidOperationException("Configura Authentication:Authority HTTPS y Authentication:Audience."); // No permite acceso anónimo accidental.
builder.Services.AddDbContext<TallerDbContext>(options => options.UseSqlServer(connection, sql => sql.CommandTimeout(30))); // Contexto scoped y timeout SQL.
builder.Services.AddScoped<IClienteRepository, ClienteRepository>(); // Inversión de dependencia.
builder.Services.AddScoped<ClienteService>(); // Servicio por solicitud.
builder.Services.AddExceptionHandler<ApiExceptionHandler>(); // Errores sanitizados.
builder.Services.AddProblemDetails(); // Infraestructura del manejador global.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => // Personaliza validación automática.
{
    options.SuppressMapClientErrors = true; // Evita otro formato de errores.
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult( // Conserva HTTP 400.
        new ResponseWrapper<object>(HttpStatusCode.BadRequest, "Solicitud inválida.",
            context.ModelState.Where(x => x.Value?.Errors.Count > 0).ToDictionary(x => x.Key, _ => new[] { "Valor inválido o requerido." }))); // No refleja valores ni errores del parser.
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => // Valida access tokens externos.
{
    options.Authority = authority; // Descubrimiento de claves públicas.
    options.Audience = audience; // Token dirigido a esta API.
    options.RequireHttpsMetadata = true; // Solo descubrimiento cifrado.
    options.MapInboundClaims = false; // Conserva nombres originales.
    options.IncludeErrorDetails = false; // No expone detalles criptográficos.
    options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30); // Tolerancia temporal acotada.
    options.TokenValidationParameters.ValidateIssuer = true; // Valida emisor.
    options.TokenValidationParameters.ValidateAudience = true; // Valida destinatario.
    options.TokenValidationParameters.ValidateLifetime = true; // Rechaza expiración.
    options.TokenValidationParameters.RequireSignedTokens = true; // Rechaza tokens sin firma.
});
builder.Services.AddAuthorization(options => options.AddPolicy("CanManageClientes", policy => // Política especificada.
    policy.RequireAuthenticatedUser().RequireClaim("permission", "clientes.manage"))); // Permiso explícito.
builder.Services.AddRateLimiter(options => // Límites por instancia; complementar en gateway distribuido.
{
    options.GlobalLimiter = PartitionedRateLimiter.CreateChained( // Presupuesto de concurrencia y frecuencia.
        PartitionedRateLimiter.Create<HttpContext, string>(_ => RateLimitPartition.GetConcurrencyLimiter("concurrency", _ => // Clave fija evita cardinalidad ilimitada.
            new ConcurrencyLimiterOptions { PermitLimit = 64, QueueLimit = 0 })), // Sin cola de saturación.
        PartitionedRateLimiter.Create<HttpContext, string>(_ => RateLimitPartition.GetFixedWindowLimiter("requests", _ => // Límite agregado.
            new FixedWindowRateLimiterOptions { PermitLimit = 600, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }))); // Ajustar con mediciones.
    options.OnRejected = async (context, token) => // Error JSON uniforme.
    {
        context.HttpContext.Response.StatusCode = 429; // Demasiadas solicitudes.
        await context.HttpContext.Response.WriteAsJsonAsync(new ResponseWrapper<object>((HttpStatusCode)429, "Límite de solicitudes excedido.", null), token); // Sin datos internos.
    };
});
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, context, token) => // Enriquece el contrato sin paquetes adicionales.
{
    document.Components ??= new OpenApiComponents(); // Inicializa componentes si no hay esquemas.
    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme // Access token del proveedor externo.
    {
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", // Esquema estándar HTTP.
        Description = "Access token con claim permission=clientes.manage." // Explica el permiso necesario.
    };
    foreach (var path in document.Paths.Where(x => x.Key.EndsWith("/clientes", StringComparison.Ordinal))) // Documenta ambos alias del recurso.
    foreach (var operation in path.Value.Operations) // GET y POST.
    {
        operation.Value.OperationId = operation.Key + (path.Key.Contains("/v1/") ? "ClientesV1" : "Clientes"); // Identificadores únicos para generadores de clientes.
        operation.Value.Security = [new OpenApiSecurityRequirement // Exige Bearer en documentación.
        {
            [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>() // HTTP Bearer no utiliza scopes OpenAPI.
        }];
    }
    return Task.CompletedTask; // Transformación local sin I/O.
}));
var app = builder.Build(); // Construye el servidor.
app.Use(async (context, next) => // Correlación controlada por el servidor.
{
    context.TraceIdentifier = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N"); // No confía en cabeceras arbitrarias.
    context.Response.Headers["X-Correlation-ID"] = context.TraceIdentifier; // Identifica la solicitud.
    context.Response.Headers["X-Content-Type-Options"] = "nosniff"; // Impide interpretación incorrecta del contenido.
    context.Response.Headers.CacheControl = "no-store"; // Protege datos personales.
    await next(context); // Continúa el pipeline.
});
app.UseExceptionHandler(); // Captura errores del pipeline siguiente.
app.UseStatusCodePages(async context => // Completa errores sin cuerpo, incluidos 401 y 403.
{
    var response = context.HttpContext.Response; // Estado determinado por ASP.NET.
    await response.WriteAsJsonAsync(new ResponseWrapper<object>((HttpStatusCode)response.StatusCode, "La solicitud no pudo completarse.", null)); // Alinea sobre y HTTP.
});
if (!app.Environment.IsDevelopment()) app.UseHsts(); // HSTS fuera de desarrollo.
app.UseHttpsRedirection(); // Exige transporte cifrado al cliente.
app.UseRouting(); // Resuelve endpoint.
app.UseRateLimiter(); // Protege incluso antes del trabajo criptográfico.
app.UseAuthentication(); // Valida identidad.
app.UseAuthorization(); // Evalúa permisos.
if (app.Environment.IsDevelopment()) app.MapOpenApi(); // Solo publica documentación en desarrollo.
app.MapControllers(); // Publica clientes.
app.Run(); // No migra ni siembra automáticamente.
public partial class Program { } // Permite pruebas de integración con WebApplicationFactory.
