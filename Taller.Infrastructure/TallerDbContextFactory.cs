using Microsoft.Data.SqlClient; // Construye y escapa correctamente los valores de la conexión SQL Server.
using Microsoft.EntityFrameworkCore; // Configura el proveedor SQL Server.
using Microsoft.EntityFrameworkCore.Design; // Activación exclusiva de herramientas EF.

namespace Taller.Infrastructure; // Infraestructura de persistencia.

/// <summary>Centraliza la conexión de las herramientas EF usando variables de entorno obligatorias.</summary>
public sealed class TallerDbContextFactory : IDesignTimeDbContextFactory<TallerDbContext>
{
    public TallerDbContext CreateDbContext(string[] args)
    {
        var connection = BuildConnectionString(); // Lee la configuración vigente del proceso al crear el contexto.
        return new(new DbContextOptionsBuilder<TallerDbContext>().UseSqlServer(connection).Options); // Configura EF sin abrir todavía la conexión.
    }

    /// <summary>Forma la cadena sin concatenaciones ni credenciales predeterminadas.</summary>
    public static string BuildConnectionString()
    {
        var connection = new SqlConnectionStringBuilder // Escapa caracteres especiales como punto y coma o comillas.
        {
            DataSource = GetRequiredEnvironmentVariable("TALLER_DB_SERVER"), // Servidor SQL; admite servidor,puerto.
            InitialCatalog = GetRequiredEnvironmentVariable("TALLER_DB_DATABASE"), // Nombre de la base de datos.
            UserID = GetRequiredEnvironmentVariable("TALLER_DB_USER"), // Usuario de autenticación SQL Server.
            Password = GetRequiredEnvironmentVariable("TALLER_DB_PASSWORD"), // Contraseña exacta; no se recortan espacios.
            IntegratedSecurity = false, // Utiliza el usuario y la contraseña configurados.
            Encrypt = true, // Conserva el cifrado de la configuración anterior.
            TrustServerCertificate = true, // Conserva el comportamiento TLS previo de esta clase.
            PersistSecurityInfo = false // Evita conservar información sensible al abrir una conexión.
        };
        return connection.ConnectionString; // Devuelve la cadena correctamente formada, sin registrarla en logs.
    }

    /// <summary>Rechaza configuración incompleta e informa únicamente el nombre de la variable.</summary>
    private static string GetRequiredEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name); // Lee el entorno heredado por el proceso.
        if (string.IsNullOrWhiteSpace(value)) // No permite valores ausentes, vacíos o compuestos solo por espacios.
            throw new InvalidOperationException($"La variable de entorno '{name}' es obligatoria."); // No expone credenciales.
        return value; // Preserva el valor original, incluidos caracteres especiales de la contraseña.
    }
}
