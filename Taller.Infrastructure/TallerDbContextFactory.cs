using Microsoft.EntityFrameworkCore; // Configura el proveedor SQL Server.
using Microsoft.EntityFrameworkCore.Design; // Activación exclusiva de herramientas EF.
namespace Taller.Infrastructure; // Infraestructura de persistencia.
/// <summary>Permite migraciones sin iniciar HTTP ni configurar el proveedor de identidad.</summary>
public sealed class TallerDbContextFactory : IDesignTimeDbContextFactory<TallerDbContext>
{
    public TallerDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__Taller") // Usa la base explícita del despliegue.
            ?? @"Server=localhost,14033;Database=TallerMecanicoDev;User Id=sa;Password=Sistemas410;Integrated Security=False;Encrypt=True;TrustServerCertificate=True"; // Solo fallback local de herramientas.
        return new(new DbContextOptionsBuilder<TallerDbContext>().UseSqlServer(connection).Options); // No conecta hasta ejecutar una operación.
    }
}
//var connection = Environment.GetEnvironmentVariable("ConnectionStrings__Taller") // Usa la base explícita del despliegue.
//?? @"Server=172.27.27.85;Port=1433;;Database=TallerMecanicoDev;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"; // Solo fallback local de herramientas.

//Server=localhost,14033;Database=TallerMecanicoDev;User Id=sa;Password=Sistemas410;Integrated Security=False;Encrypt=True;TrustServerCertificate=True