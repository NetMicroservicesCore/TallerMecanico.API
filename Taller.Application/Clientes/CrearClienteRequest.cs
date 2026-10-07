using System.ComponentModel.DataAnnotations; // Reglas de entrada reutilizables.
namespace Taller.Application.Clientes; // Contratos de aplicación.

/// <summary>Datos aceptados para registrar un cliente; no permite asignar Id ni fecha.</summary>
public sealed class CrearClienteRequest
{
    [Required] // Nombre del cliente requerido; rechaza valores vacíos o espacios.
    [StringLength(100)] // Longitud consistente con la columna SQL.
    public string Nombre { get; init; } = string.Empty; // Nombre del cliente.
    [Required] // Apellido paterno requerido; rechaza valores vacíos o espacios.
    [StringLength(100)] // Longitud consistente con la columna SQL.
    public string ApellidoPaterno { get; init; } = string.Empty; // Apellido paterno.
    [StringLength(100)] // Longitud consistente con la columna SQL.
    public string? ApellidoMaterno { get; init; } // Apellido materno opcional.
    [Required] // Teléfono fijo requerido; rechaza valores vacíos o espacios.
    [StringLength(25)] // Longitud consistente con la columna SQL.
    [RegularExpression(@"^\+?[0-9][0-9 ()-]{6,24}$")] // Admite prefijo internacional y formato legible.
    public string Telefono { get; init; } = string.Empty; // Teléfono fijo.
    [Required] // Teléfono celular requerido; rechaza valores vacíos o espacios.
    [StringLength(25)] // Longitud consistente con la columna SQL.
    [RegularExpression(@"^\+?[0-9][0-9 ()-]{6,24}$")] // Admite prefijo internacional y formato legible.
    public string TelefonoCelular { get; init; } = string.Empty; // Teléfono celular.
    [Required] // Correo del cliente requerido; rechaza valores vacíos o espacios.
    [StringLength(254)] // Longitud consistente con la columna SQL.
    [EmailAddress] // Rechaza sintaxis de correo inválida.
    public string Email { get; init; } = string.Empty; // Correo del cliente.
    [Required] // Teléfono del contacto requerido; rechaza valores vacíos o espacios.
    [StringLength(25)] // Longitud consistente con la columna SQL.
    [RegularExpression(@"^\+?[0-9][0-9 ()-]{6,24}$")] // Admite prefijo internacional y formato legible.
    public string TelefonoContacto { get; init; } = string.Empty; // Teléfono del contacto.
    [Required] // Nombre completo del contacto requerido; rechaza valores vacíos o espacios.
    [StringLength(200)] // Longitud consistente con la columna SQL.
    public string NombreCompletoContacto { get; init; } = string.Empty; // Nombre completo del contacto.
    [Required] // Correo del contacto requerido; rechaza valores vacíos o espacios.
    [StringLength(254)] // Longitud consistente con la columna SQL.
    [EmailAddress] // Rechaza sintaxis de correo inválida.
    public string EmailContacto { get; init; } = string.Empty; // Correo del contacto.
    [Required] // Calle y número requerido; rechaza valores vacíos o espacios.
    [StringLength(200)] // Longitud consistente con la columna SQL.
    public string Calle { get; init; } = string.Empty; // Calle y número.
    [Required] // Colonia sin duplicar el campo requerido; rechaza valores vacíos o espacios.
    [StringLength(150)] // Longitud consistente con la columna SQL.
    public string Colonia { get; init; } = string.Empty; // Colonia sin duplicar el campo.
    [Required] // Municipio de residencia requerido; rechaza valores vacíos o espacios.
    [StringLength(150)] // Longitud consistente con la columna SQL.
    public string Municipio { get; init; } = string.Empty; // Municipio de residencia.
    [Required] // Estado de residencia requerido; rechaza valores vacíos o espacios.
    [StringLength(100)] // Longitud consistente con la columna SQL.
    public string Estado { get; init; } = string.Empty; // Estado de residencia.
    [Required] // Código postal mexicano de cinco dígitos requerido; rechaza valores vacíos o espacios.
    [StringLength(5)] // Longitud consistente con la columna SQL.
    [RegularExpression(@"^[0-9]{5}$")] // Conserva ceros iniciales del código postal.
    public string CodigoPostal { get; init; } = string.Empty; // Código postal mexicano de cinco dígitos.
}
