using System.ComponentModel.DataAnnotations; // Validación declarativa del contrato de consulta.
namespace Taller.Application.Clientes; // Contratos del módulo de clientes.

/// <summary>Consulta acotada para evitar respuestas y desplazamientos ilimitados.</summary>
public sealed class ClientesQuery
{
    [Range(1, 10000)] // Impide páginas inválidas y offsets excesivos.
    public int PageNumber { get; init; } = 1; // Primera página por defecto.
    [Range(1, 100)] // Limita memoria, serialización y trabajo de la base.
    public int PageSize { get; init; } = 20; // Tamaño conservador por defecto.
    [Required, RegularExpression("^(asc|desc)$")] // Lista permitida, sin SQL dinámico.
    public string SortDirection { get; init; } = "asc"; // Orden ascendente por nombre.
}

/// <summary>Datos y metadatos de una página; el total corresponde al momento del conteo.</summary>
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); // Evita división entera.
    public bool HasNextPage => PageNumber < TotalPages; // Permite navegar sin inferencias del consumidor.
    public bool HasPreviousPage => PageNumber > 1; // Señala si existe una página anterior.
}
