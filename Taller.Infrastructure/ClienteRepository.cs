using Microsoft.EntityFrameworkCore; // Operadores SQL asíncronos.
using Taller.Application.Clientes; // Puerto y DTOs.
using Taller.Domain; // Entidad de escritura.
namespace Taller.Infrastructure; // Adaptador SQL Server.
/// <summary>Consultas acotadas en servidor y escritura transaccional.</summary>
public sealed class ClienteRepository(TallerDbContext db) : IClienteRepository
{
    public async Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken)
    {
        var clientes = db.Clientes.AsNoTracking(); // Evita seguimiento en lecturas.
        var total = await clientes.CountAsync(cancellationToken); // Cuenta sin materializar registros.
        var ordered = query.SortDirection == "desc" // Lista permitida, sin SQL dinámico.
            ? clientes.OrderByDescending(x => x.Nombre).ThenByDescending(x => x.Id) // Desempate estable.
            : clientes.OrderBy(x => x.Nombre).ThenBy(x => x.Id); // Usa el índice compuesto.
        var items = await ordered.Skip((query.PageNumber - 1) * query.PageSize) // Offset acotado por validación.
            .Take(query.PageSize) // Limita filas transmitidas.
            .Select(ClienteDto.Projection) // Proyecta el contrato en SQL.
            .ToListAsync(cancellationToken); // Libera el hilo durante I/O.
        return new(items, query.PageNumber, query.PageSize, total); // Adjunta navegación.
    }
    public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        db.Clientes.Add(cliente); // Prepara inserción sin consultar SQL.
        await db.SaveChangesAsync(cancellationToken); // Confirma atómicamente sin reintentos ciegos.
    }
}
