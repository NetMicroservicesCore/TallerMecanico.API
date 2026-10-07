namespace Taller.Domain; // Modelo sin dependencias de infraestructura.

/// <summary>Cliente persistido; los datos se normalizan y validan en el caso de uso.</summary>
public sealed class Cliente
{
    public Guid Id { get; init; } = Guid.NewGuid(); // Identificador generado antes de persistir.
    public DateTimeOffset CreadoUtc { get; init; } = DateTimeOffset.UtcNow; // Momento de registro en UTC.
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
}
