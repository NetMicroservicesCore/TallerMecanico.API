using System.Net; // Estados HTTP del sobre.
using API.Principal.Contracts; // Wrappers de transporte.
using Microsoft.AspNetCore.Authorization; // Políticas.
using Microsoft.AspNetCore.Mvc; // Contratos HTTP y OpenAPI.
using Taller.Application.Clientes; // Casos de uso.
namespace API.Principal.Controllers; // Adaptador HTTP.
/// <summary>Registro y listado paginado de clientes.</summary>
[ApiController] // Valida automáticamente cuerpo y query.
[Route("api/clientes")] // Ruta solicitada.
[Route("api/v1/clientes")] // Alias compatible con api.md.
[Authorize(Policy = "CanManageClientes")] // Exige token y permiso.
[Produces("application/json")] // Contrato JSON.
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // Protege datos personales.
[ProducesResponseType<ResponseWrapper<object>>(400)] // Entrada inválida.
[ProducesResponseType<ResponseWrapper<object>>(401)] // Sin autenticación.
[ProducesResponseType<ResponseWrapper<object>>(403)] // Sin permiso.
[ProducesResponseType<ResponseWrapper<object>>(429)] // Saturación.
[ProducesResponseType<ResponseWrapper<object>>(500)] // Fallo sanitizado.
public sealed class ClientesController(ClienteService service) : ControllerBase
{
    /// <summary>Ordena por nombre e Id con paginación acotada.</summary>
    [HttpGet] // Consulta sin cuerpo.
    [EndpointSummary("Consultar clientes paginados")] // Documentación OpenAPI.
    [ProducesResponseType<ResponseWrapper<PagedResponse<ClienteDto>>>(200)] // Contrato de éxito.
    public async Task<IActionResult> Get([FromQuery] ClientesQuery query, CancellationToken cancellationToken)
    {
        var page = await service.ListAsync(query, cancellationToken); // Propaga cancelación.
        return Ok(new ResponseWrapper<PagedResponse<ClienteDto>>(HttpStatusCode.OK, "Clientes consultados.", page)); // Sobre y HTTP 200.
    }
    /// <summary>Valida y registra un cliente.</summary>
    [HttpPost] // Alta explícita.
    [Consumes("application/json")] // Rechaza otros formatos.
    [EndpointSummary("Registrar cliente")] // Documentación OpenAPI.
    [ProducesResponseType<ResponseWrapper<ClienteDto>>(201)] // Alta confirmada.
    [ProducesResponseType<ResponseWrapper<object>>(415)] // Formato no soportado.
    public async Task<IActionResult> Post([FromBody] RequestWrapper<CrearClienteRequest> request, CancellationToken cancellationToken)
    {
        var cliente = await service.CreateAsync(request.Data, cancellationToken); // Delega persistencia.
        return StatusCode(201, new ResponseWrapper<ClienteDto>(HttpStatusCode.Created, "Cliente registrado.", cliente)); // No inventa un GET por Id inexistente.
    }
}
