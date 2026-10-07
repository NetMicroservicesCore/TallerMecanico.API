using System.Linq.Expressions; // Proyección traducible a SQL.
using Taller.Domain; // Modelo de persistencia interno.
namespace Taller.Application.Clientes; // Datos públicos del caso de uso.

/// <summary>Respuesta explícita que evita exponer entidades de dominio.</summary>
public sealed class ClienteDto
{
    public Guid Id { get; init; } // Identificador público.
    public DateTimeOffset CreadoUtc { get; init; } // Registro UTC.
    public string Nombre { get; init; } = string.Empty; // Nombre del cliente.
    public string ApellidoPaterno { get; init; } = string.Empty; // Apellido paterno.
    public string? ApellidoMaterno { get; init; } // Apellido materno opcional.
    public string Telefono { get; init; } = string.Empty; // Teléfono fijo.
    public string TelefonoCelular { get; init; } = string.Empty; // Teléfono celular.
    public string Email { get; init; } = string.Empty; // Correo del cliente.
    public string TelefonoContacto { get; init; } = string.Empty; // Teléfono del contacto.
    public string NombreCompletoContacto { get; init; } = string.Empty; // Nombre completo del contacto.
    public string EmailContacto { get; init; } = string.Empty; // Correo del contacto.
    public string Calle { get; init; } = string.Empty; // Calle y número.
    public string Colonia { get; init; } = string.Empty; // Colonia sin duplicar el campo.
    public string Municipio { get; init; } = string.Empty; // Municipio de residencia.
    public string Estado { get; init; } = string.Empty; // Estado de residencia.
    public string CodigoPostal { get; init; } = string.Empty; // Código postal mexicano de cinco dígitos.

    public static readonly Expression<Func<Cliente, ClienteDto>> Projection = cliente => new ClienteDto // EF ejecuta esta proyección en SQL.
    {
        Id = cliente.Id, // Expone el identificador.
        CreadoUtc = cliente.CreadoUtc, // Expone la fecha de registro.
        Nombre = cliente.Nombre, // Copia nombre del cliente.
        ApellidoPaterno = cliente.ApellidoPaterno, // Copia apellido paterno.
        ApellidoMaterno = cliente.ApellidoMaterno, // Copia apellido materno opcional.
        Telefono = cliente.Telefono, // Copia teléfono fijo.
        TelefonoCelular = cliente.TelefonoCelular, // Copia teléfono celular.
        Email = cliente.Email, // Copia correo del cliente.
        TelefonoContacto = cliente.TelefonoContacto, // Copia teléfono del contacto.
        NombreCompletoContacto = cliente.NombreCompletoContacto, // Copia nombre completo del contacto.
        EmailContacto = cliente.EmailContacto, // Copia correo del contacto.
        Calle = cliente.Calle, // Copia calle y número.
        Colonia = cliente.Colonia, // Copia colonia sin duplicar el campo.
        Municipio = cliente.Municipio, // Copia municipio de residencia.
        Estado = cliente.Estado, // Copia estado de residencia.
        CodigoPostal = cliente.CodigoPostal, // Copia código postal mexicano de cinco dígitos.
    };
}
