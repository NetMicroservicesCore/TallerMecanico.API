using System.ComponentModel.DataAnnotations; // Validación también fuera del controlador.
using Taller.Domain; // Entidad del módulo.
namespace Taller.Application.Clientes; // Casos de uso independientes de HTTP.

/// <summary>Coordina validación, normalización y persistencia.</summary>
public sealed class ClienteService(IClienteRepository repository)
{
    public Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken) // Propaga cancelación.
    {
        Validator.ValidateObject(query, new ValidationContext(query), true); // Protege llamadas fuera de HTTP.
        return repository.ListAsync(query, cancellationToken); // Delega consulta al puerto.
    }

    public async Task<ClienteDto> CreateAsync(CrearClienteRequest request, CancellationToken cancellationToken) // Caso de uso de alta.
    {
        Validator.ValidateObject(request, new ValidationContext(request), true); // Valida todos los atributos.
        var cliente = new Cliente // Asigna únicamente campos autorizados.
        {
            Nombre = request.Nombre.Trim(), // Normaliza bordes sin modificar el contenido.
            ApellidoPaterno = request.ApellidoPaterno.Trim(), // Normaliza bordes sin modificar el contenido.
            ApellidoMaterno = string.IsNullOrWhiteSpace(request.ApellidoMaterno) ? null : request.ApellidoMaterno.Trim(), // Normaliza bordes sin modificar el contenido.
            Telefono = request.Telefono.Trim(), // Normaliza bordes sin modificar el contenido.
            TelefonoCelular = request.TelefonoCelular.Trim(), // Normaliza bordes sin modificar el contenido.
            Email = request.Email.Trim(), // Normaliza bordes sin modificar el contenido.
            TelefonoContacto = request.TelefonoContacto.Trim(), // Normaliza bordes sin modificar el contenido.
            NombreCompletoContacto = request.NombreCompletoContacto.Trim(), // Normaliza bordes sin modificar el contenido.
            EmailContacto = request.EmailContacto.Trim(), // Normaliza bordes sin modificar el contenido.
            Calle = request.Calle.Trim(), // Normaliza bordes sin modificar el contenido.
            Colonia = request.Colonia.Trim(), // Normaliza bordes sin modificar el contenido.
            Municipio = request.Municipio.Trim(), // Normaliza bordes sin modificar el contenido.
            Estado = request.Estado.Trim(), // Normaliza bordes sin modificar el contenido.
            CodigoPostal = request.CodigoPostal.Trim(), // Normaliza bordes sin modificar el contenido.
        };
        await repository.AddAsync(cliente, cancellationToken); // Confirma antes de responder éxito.
        return Map(cliente); // Nunca devuelve la entidad EF.
    }

    private static readonly Func<Cliente, ClienteDto> Map = ClienteDto.Projection.Compile(); // Compila una vez el mapeo de escritura.
}
