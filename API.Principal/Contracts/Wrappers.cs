using System.ComponentModel.DataAnnotations; // Validación del cuerpo exterior.
using System.Net; // Código HTTP fuertemente tipado.
namespace API.Principal.Contracts; // Contratos exclusivos del transporte HTTP.

/// <summary>La entrada solo contiene datos; el consumidor no decide el estado HTTP.</summary>
public sealed class RequestWrapper<T> where T : class
{
    [Required] // Rechaza cuerpos sin la propiedad data o con data null.
    public required T Data { get; init; } // ASP.NET también valida el objeto anidado.
}

/// <summary>Sobre uniforme; StatusCode se serializa como número y coincide con HTTP.</summary>
public sealed record ResponseWrapper<T>(HttpStatusCode StatusCode, string Message, T? Data); // Modelo inmutable.
