using System.Net; // Estados esperados.
using System.Net.Http.Headers; // Cabecera Bearer.
using System.Net.Http.Json; // JSON HTTP.
using System.Text; // Cuerpos inválidos.
using System.Text.Json; // Inspección de contratos.
using Xunit; // Aserciones y casos de prueba.
using Microsoft.AspNetCore.TestHost; // Configuración aislada para fallos deliberados.
using Microsoft.Extensions.DependencyInjection; // Sustitución controlada de un puerto en prueba negativa.
using Taller.Application.Clientes; // Contratos del adaptador que falla.
using Taller.Domain; // Tipo del puerto de escritura.
namespace Taller.Tests; // Integración API y SQL Server.
/// <summary>Verifica comportamiento observable sin reemplazar repositorio, EF ni autenticación.</summary>
public sealed class ClientesTests(ClientesFactory factory) : IClassFixture<ClientesFactory>
{
    private HttpRequestMessage Request(HttpMethod method, string path, string? token = null, object? body = null)
    {
        var request = new HttpRequestMessage(method, path); // Crea solicitud aislada.
        if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // No comparte cabeceras mutables.
        if (body is not null) request.Content = JsonContent.Create(body); // Usa JSON real.
        return request; // El llamador dispone la solicitud.
    }
    private static object Body(string nombre = "Ana", string email = "ana@example.test") => new // Datos ficticios sin PII real.
    {
        data = new { nombre, apellidoPaterno = "Perez", apellidoMaterno = "Lopez", telefono = "5551234567", telefonoCelular = "+525551234568", email,
            telefonoContacto = "5551234569", nombreCompletoContacto = "Contacto Prueba", emailContacto = "contacto@example.test", calle = "Calle 1", colonia = "Centro", municipio = "Puebla", estado = "Puebla", codigoPostal = "01234" } // Contrato completo.
    };
    [Theory] // Asegura protección en ambos alias y verbos.
    [InlineData("GET", "/api/clientes")]
    [InlineData("POST", "/api/clientes")]
    [InlineData("GET", "/api/v1/clientes")]
    [InlineData("POST", "/api/v1/clientes")]
    public async Task SinTokenDevuelve401(string method, string path)
    {
        using var request = Request(new HttpMethod(method), path, body: method == "POST" ? Body() : null); // Sin credenciales.
        using var response = await factory.Client.SendAsync(request); // Ejecuta pipeline.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // No accede al caso de uso.
        Assert.Equal(401, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Wrapper uniforme.
    }
    [Fact] // Token válido sin autorización.
    public async Task SinPermisoDevuelve403()
    {
        using var request = Request(HttpMethod.Get, "/api/clientes", factory.Token(permission: false)); // Identidad sin permiso.
        using var response = await factory.Client.SendAsync(request); // Autenticación real.
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode); // Política efectiva.
    }
    [Theory] // Rechaza tokens inválidos criptográfica o semánticamente.
    [InlineData(true, "taller-tests", false)]
    [InlineData(false, "otra-api", false)]
    [InlineData(false, "taller-tests", true)]
    public async Task TokenInvalidoDevuelve401(bool expired, string audience, bool badSignature)
    {
        using var request = Request(HttpMethod.Get, "/api/clientes", factory.Token(expired: expired, audience: audience, badSignature: badSignature)); // Construye caso negativo.
        using var response = await factory.Client.SendAsync(request); // Verifica el validador productivo.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Ningún token inválido entra.
    }
    [Theory] // Límites y allowlist de orden.
    [InlineData("pageNumber=0")]
    [InlineData("pageNumber=10001")]
    [InlineData("pageSize=101")]
    [InlineData("pageSize=-1")]
    [InlineData("sortDirection=drop")]
    public async Task QueryInvalidaDevuelve400(string query)
    {
        using var request = Request(HttpMethod.Get, "/api/clientes?" + query, factory.Token()); // Query inválida.
        using var response = await factory.Client.SendAsync(request); // Model binding real.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Rechaza antes de consultar.
    }
    [Fact] // Valida objetos anidados.
    public async Task EmailInvalidoDevuelve400()
    {
        using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token(), Body(email: "invalido")); // Formato incorrecto.
        using var response = await factory.Client.SendAsync(request); // Validación del DTO interno.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // No persiste.
    }
    [Theory] // Cuerpos ausentes, vacíos y malformados.
    [InlineData("{}")]
    [InlineData("{\"data\":null}")]
    [InlineData("{\"data\":{}}")]
    [InlineData("{no-json")]
    public async Task CuerpoInvalidoDevuelve400(string body)
    {
        using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token()); // Token autorizado.
        request.Content = new StringContent(body, Encoding.UTF8, "application/json"); // Conserva cuerpo inválido.
        using var response = await factory.Client.SendAsync(request); // Deserialización real.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // No produce un 500.
    }
    [Fact] // Flujo funcional persistido y ordenado.
    public async Task RegistroConsultaOrdenYPaginacion()
    {
        foreach (var nombre in new[] { "Zoe", "Ana", "Ana", "Luis" }) // Incluye empate por nombre.
        {
            using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token(), Body(nombre)); // Alta real.
            using var response = await factory.Client.SendAsync(request); // Guarda en SQL Server.
            Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Confirma creación.
            var json = await response.Content.ReadFromJsonAsync<JsonElement>(); // Inspecciona DTO.
            Assert.Equal(nombre, json.GetProperty("data").GetProperty("nombre").GetString()); // Mantiene datos.
            Assert.Equal("01234", json.GetProperty("data").GetProperty("codigoPostal").GetString()); // Preserva cero inicial.
            Assert.True(response.Headers.Contains("X-Correlation-ID")); // Trazabilidad.
        }
        var ids = new List<string>(); // Acumula páginas para detectar solapamientos.
        foreach (var page in new[] { 1, 2 }) // Recorre dos páginas consecutivas.
        {
            using var request = Request(HttpMethod.Get, $"/api/v1/clientes?pageNumber={page}&pageSize=2&sortDirection=asc", factory.Token()); // Alias versionado.
            using var response = await factory.Client.SendAsync(request); // Consulta SQL paginada.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Éxito.
            var data = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data"); // Sobre paginado.
            Assert.Equal(4, data.GetProperty("totalCount").GetInt32()); // Conteo persistido.
            Assert.Equal(2, data.GetProperty("totalPages").GetInt32()); // Metadatos correctos.
            ids.AddRange(data.GetProperty("items").EnumerateArray().Select(x => x.GetProperty("id").GetString()!)); // Identificadores por página.
            Assert.Equal(page == 1 ? "Ana" : "Luis", data.GetProperty("items")[0].GetProperty("nombre").GetString()); // Orden esperado.
        }
        Assert.Equal(4, ids.Distinct().Count()); // Sin duplicados entre páginas estables.
        using var descending = Request(HttpMethod.Get, "/api/clientes?pageSize=4&sortDirection=desc", factory.Token()); // Invierte todo el orden.
        using var descendingResponse = await factory.Client.SendAsync(descending); // Consulta inversa.
        var reverse = (await descendingResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data").GetProperty("items"); // Resultado descendente.
        Assert.Equal(ids.AsEnumerable().Reverse(), reverse.EnumerateArray().Select(x => x.GetProperty("id").GetString()!)); // Comprueba desempate inverso.
        using var beyond = Request(HttpMethod.Get, "/api/clientes?pageNumber=100", factory.Token()); // Página sin filas.
        using var beyondResponse = await factory.Client.SendAsync(beyond); // Debe ser éxito vacío.
        Assert.Empty((await beyondResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data").GetProperty("items").EnumerateArray()); // Nunca 404 por página vacía.
    }
    [Fact] // Verifica exposición y documentación.
    public async Task OpenApiDocumentaLosDosAlias()
    {
        var document = await factory.Client.GetFromJsonAsync<JsonElement>("/openapi/v1.json"); // Documento generado.
        foreach (var path in new[] { "/api/clientes", "/api/v1/clientes" }) // Dos rutas compatibles.
        {
            var item = document.GetProperty("paths").GetProperty(path); // Operaciones publicadas.
            Assert.True(item.TryGetProperty("get", out _)); // Consulta documentada.
            Assert.True(item.TryGetProperty("post", out _)); // Alta documentada.
            Assert.True(item.GetProperty("get").TryGetProperty("security", out _)); // Bearer documentado.
        }
    }
    [Fact] // Un formato distinto de JSON conserva el sobre de error.
    public async Task FormatoNoSoportadoDevuelve415()
    {
        using var request = Request(HttpMethod.Post, "/api/clientes", factory.Token()); // Usuario autorizado.
        request.Content = new StringContent("texto", Encoding.UTF8, "text/plain"); // Formato no soportado.
        using var response = await factory.Client.SendAsync(request); // Ejecuta selección del formatter.
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode); // HTTP 415.
        Assert.Equal(415, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Sobre consistente.
    }
    [Fact] // Prueba manejo de fallos sin alterar la base real del fixture.
    public async Task FalloInternoNoExponeDetalles()
    {
        await using var failing = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddScoped<IClienteRepository, FailingRepository>())); // Puerto fallido solo en este host.
        using var client = failing.CreateClient(new() { BaseAddress = new Uri("https://localhost") }); // Host independiente.
        using var request = Request(HttpMethod.Get, "/api/clientes", factory.Token()); // Token válido.
        using var response = await client.SendAsync(request); // Provoca fallo tras autorización.
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode); // HTTP 500.
        Assert.DoesNotContain("dato-secreto", await response.Content.ReadAsStringAsync()); // Sanitización observable.
        Assert.Equal(500, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Wrapper correcto.
    }
    [Fact] // Límite activo incluso para solicitudes sin token.
    public async Task SaturacionDevuelve429()
    {
        await using var isolated = factory.WithWebHostBuilder(_ => { }); // Presupuesto separado del resto de pruebas.
        using var client = isolated.CreateClient(new() { BaseAddress = new Uri("https://localhost") }); // Solicitudes HTTPS.
        for (var index = 0; index <= 600; index++) // Supera la ventana sin tocar SQL.
        {
            using var response = await client.GetAsync("/api/clientes"); // Solicitud anónima contabilizada.
            if (index == 600) // Primera solicitud fuera del presupuesto.
            {
                Assert.Equal((HttpStatusCode)429, response.StatusCode); // Protección efectiva.
                Assert.Equal(429, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("statusCode").GetInt32()); // Sobre uniforme.
            }
        }
    }
    private sealed class FailingRepository : IClienteRepository // Adaptador negativo exclusivo de pruebas.
    {
        public Task<PagedResponse<ClienteDto>> ListAsync(ClientesQuery query, CancellationToken cancellationToken) => throw new InvalidOperationException("dato-secreto"); // Simula detalle sensible.
        public Task AddAsync(Cliente cliente, CancellationToken cancellationToken) => throw new InvalidOperationException("dato-secreto"); // Nunca ejecuta SQL.
    }
}
