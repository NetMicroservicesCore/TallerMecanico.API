using Taller.Domain; // Entidad independiente de EF Core.
namespace Taller.Application.Clientes; // Puerto de persistencia del módulo.

/// <summary>Operaciones específicas; no expone IQueryable ni detalles del proveedor.</summary>
public interface IClienteRepository
{
    Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken); // Consulta proyectada y acotada.
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken); // Guarda una unidad de trabajo atómica.
}
