using Microsoft.EntityFrameworkCore; // Mapeo relacional.
using Taller.Domain; // Entidad persistida.
namespace Taller.Infrastructure; // Adaptadores externos.

/// <summary>Unidad de trabajo scoped; no se comparte entre solicitudes.</summary>
public sealed class TallerDbContext(DbContextOptions<TallerDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>(); // Acceso tipado al agregado.
    protected override void OnModelCreating(ModelBuilder modelBuilder) // Define esquema sin anotaciones en dominio.
    {
        var cliente = modelBuilder.Entity<Cliente>(); // Configura la única entidad inicial.
        cliente.ToTable("Clientes"); // Nombre estable de tabla.
        cliente.HasKey(x => x.Id); // Clave primaria.
        cliente.Property(x => x.Id).ValueGeneratedNever(); // El dominio genera el UUID.
        cliente.HasIndex(x => new { x.Nombre, x.Id }); // Soporta orden determinista y paginado.
        cliente.Property(x => x.Nombre).HasMaxLength(100).IsRequired(); // Limita nombre del cliente.
        cliente.Property(x => x.ApellidoPaterno).HasMaxLength(100).IsRequired(); // Limita apellido paterno.
        cliente.Property(x => x.ApellidoMaterno).HasMaxLength(100); // Limita apellido materno opcional.
        cliente.Property(x => x.Telefono).HasMaxLength(25).IsRequired(); // Limita teléfono fijo.
        cliente.Property(x => x.TelefonoCelular).HasMaxLength(25).IsRequired(); // Limita teléfono celular.
        cliente.Property(x => x.Email).HasMaxLength(254).IsRequired(); // Limita correo del cliente.
        cliente.Property(x => x.TelefonoContacto).HasMaxLength(25).IsRequired(); // Limita teléfono del contacto.
        cliente.Property(x => x.NombreCompletoContacto).HasMaxLength(200).IsRequired(); // Limita nombre completo del contacto.
        cliente.Property(x => x.EmailContacto).HasMaxLength(254).IsRequired(); // Limita correo del contacto.
        cliente.Property(x => x.Calle).HasMaxLength(200).IsRequired(); // Limita calle y número.
        cliente.Property(x => x.Colonia).HasMaxLength(150).IsRequired(); // Limita colonia sin duplicar el campo.
        cliente.Property(x => x.Municipio).HasMaxLength(150).IsRequired(); // Limita municipio de residencia.
        cliente.Property(x => x.Estado).HasMaxLength(100).IsRequired(); // Limita estado de residencia.
        cliente.Property(x => x.CodigoPostal).HasMaxLength(5).IsRequired(); // Limita código postal mexicano de cinco dígitos.
    }
}
