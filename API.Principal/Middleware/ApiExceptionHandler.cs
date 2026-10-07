using System.Net; // Estados de error.
using API.Principal.Contracts; // Sobre uniforme.
using Microsoft.AspNetCore.Diagnostics; // Manejo global.
namespace API.Principal.Middleware; // Responsabilidades transversales.
/// <summary>Oculta detalles internos y evita registrar datos personales.</summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError("Fallo {ExceptionType}; correlación {TraceId}", exception.GetType().Name, context.TraceIdentifier); // Solo tipo y correlación.
        context.Response.StatusCode = 500; // Estado real del error.
        await context.Response.WriteAsJsonAsync(new ResponseWrapper<object>(HttpStatusCode.InternalServerError,
            "Ocurrió un error interno. Usa X-Correlation-ID para solicitar soporte.", null), cancellationToken); // Sin stack trace ni SQL.
        return true; // Marca la excepción como manejada.
    }
}
