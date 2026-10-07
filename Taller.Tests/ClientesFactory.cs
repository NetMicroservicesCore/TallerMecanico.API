using System.IdentityModel.Tokens.Jwt; // Generación exclusiva de tokens de prueba.
using System.Security.Claims; // Permisos de prueba.
using System.Security.Cryptography; // Clave efímera, nunca versionada.
using Microsoft.AspNetCore.Authentication.JwtBearer; // Mantiene autenticación real.
using Microsoft.AspNetCore.Hosting; // Personaliza el host de pruebas.
using Microsoft.AspNetCore.Mvc.Testing; // Servidor HTTP en memoria.
using Microsoft.EntityFrameworkCore; // Migraciones reales.
using Microsoft.Extensions.DependencyInjection; // Resolución scoped.
using Microsoft.IdentityModel.Protocols; // Descubrimiento estático aislado.
using Microsoft.IdentityModel.Protocols.OpenIdConnect; // Configuración de emisor de prueba.
using Microsoft.IdentityModel.Tokens; // Firma y validación criptográfica.
using Taller.Infrastructure; // DbContext productivo.
using Xunit; // Ciclo de vida asíncrono.
namespace Taller.Tests; // Pruebas aisladas del entorno productivo.
/// <summary>Usa SQL Server real con una base temporal única y JWT firmado con RSA efímero.</summary>
public sealed class ClientesFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RSA rsa = RSA.Create(2048); // Clave efímera por ejecución.
    private readonly string database = "TallerTests_" + Guid.NewGuid().ToString("N"); // Evita tocar bases existentes.
    public HttpClient Client { get; private set; } = null!; // Se inicializa después del host.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development"); // Habilita OpenAPI para verificar contrato.
        builder.UseSetting("ConnectionStrings:Taller", $@"Server=(localdb)\MSSQLLocalDB;Database={database};Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"); // Solo base temporal.
        builder.UseSetting("Authentication:Authority", "https://test-issuer.invalid"); // Emisor aislado.
        builder.UseSetting("Authentication:Audience", "taller-tests"); // Audiencia aislada.
        builder.ConfigureServices(services => services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options => // Cambia claves, no omite autenticación.
        {
            var configuration = new OpenIdConnectConfiguration { Issuer = "https://test-issuer.invalid" }; // Sin llamadas externas.
            configuration.SigningKeys.Add(new RsaSecurityKey(rsa) { KeyId = "test-key" }); // Clave pública verificadora.
            options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration); // Descubrimiento determinista.
            options.TokenValidationParameters.ValidAudience = "taller-tests"; // Rechaza otras audiencias.
        }));
    }
    public string Token(bool permission = true, bool expired = false, string audience = "taller-tests", bool badSignature = false)
    {
        using var other = RSA.Create(2048); // Clave alternativa para verificar rechazo de firmas.
        var claims = permission ? new[] { new Claim("sub", "tester"), new Claim("permission", "clientes.manage") } : new[] { new Claim("sub", "tester") }; // Permisos controlados.
        var token = new JwtSecurityToken("https://test-issuer.invalid", audience, claims, DateTime.UtcNow.AddHours(-2),
            expired ? DateTime.UtcNow.AddHours(-1) : DateTime.UtcNow.AddMinutes(5), // Prueba expiración real.
            new SigningCredentials(new RsaSecurityKey(badSignature ? other : rsa) { KeyId = "test-key" }, SecurityAlgorithms.RsaSha256)); // Firma asimétrica.
        return new JwtSecurityTokenHandler().WriteToken(token); // Token wire-format.
    }
    public async Task InitializeAsync()
    {
        Client = CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false }); // Simula HTTPS.
        using var scope = Services.CreateScope(); // DbContext independiente.
        await scope.ServiceProvider.GetRequiredService<TallerDbContext>().Database.MigrateAsync(); // Aplica migración sobre SQL Server real.
    }
    async Task IAsyncLifetime.DisposeAsync()
    {
        using var scope = Services.CreateScope(); // Limpieza de la base propiedad del fixture.
        var db = scope.ServiceProvider.GetRequiredService<TallerDbContext>(); // Proveedor real.
        if (db.Database.GetDbConnection().Database != database || !database.StartsWith("TallerTests_")) throw new InvalidOperationException("Base no autorizada para limpieza."); // Guardia contra borrado accidental.
        await db.Database.EnsureDeletedAsync(); // Elimina únicamente la base temporal única.
        Client.Dispose(); // Libera conexiones HTTP.
        await DisposeAsync(); // Cierra el servidor.
        rsa.Dispose(); // Destruye la clave efímera.
    }
}
