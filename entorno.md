# Configuración de entorno de TallerDbContextFactory

## Estado

Implementado y compilado: 0 errores y 0 advertencias al ejecutar `dotnet build Taller.Infrastructure/Taller.Infrastructure.csproj --no-restore -m:1 --nologo`. No se ha intentado conectar al servidor SQL ni ejecutar migraciones.

## Variables utilizadas

Las cuatro variables son obligatorias y deben estar disponibles en el entorno del proceso que ejecuta las herramientas de Entity Framework.

| Variable | Contenido | Propiedad SQL Server |
|---|---|---|
| `TALLER_DB_SERVER` | Nombre o dirección del servidor; para un puerto explícito, `servidor,1433` | `DataSource` |
| `TALLER_DB_DATABASE` | Nombre de la base de datos | `InitialCatalog` |
| `TALLER_DB_USER` | Nombre del usuario de SQL Server | `UserID` |
| `TALLER_DB_PASSWORD` | Contraseña del usuario, sin agregar comillas como parte del valor | `Password` |

Configurar las variables en el servidor con el alcance adecuado para la cuenta que ejecuta el proceso. Después de configurarlas o modificarlas, reiniciar el proceso o abrir una nueva terminal para que herede los valores. La clase las lee automáticamente; no requiere editar el código ni construir manualmente la cadena. No se crearon ni modificaron variables del equipo durante este cambio.

## Modificaciones realizadas en la clase

- `CreateDbContext` obtiene la cadena mediante `BuildConnectionString`.
- `BuildConnectionString` es el punto central de construcción dentro de la fábrica y utiliza `Microsoft.Data.SqlClient.SqlConnectionStringBuilder`, disponible mediante las dependencias existentes.
- Los cuatro valores se leen con `Environment.GetEnvironmentVariable`; el constructor de cadenas escapa los caracteres especiales y evita concatenaciones manuales.
- `GetRequiredEnvironmentVariable` detiene la creación del contexto si falta una variable o contiene únicamente espacios. El mensaje informa el nombre de la variable, nunca su valor.
- Se conserva íntegra la contraseña, sin aplicar `Trim`.
- Se eliminan la cadena predeterminada, las credenciales incrustadas y los ejemplos de conexión comentados. Esta clase ya no utiliza `ConnectionStrings__Taller` como alternativa.
- Se conserva la autenticación SQL (`IntegratedSecurity=false`) y la configuración previa `Encrypt=true`, `TrustServerCertificate=true`. Se explicita `PersistSecurityInfo=false`.
- No se imprime ni registra la cadena de conexión.

## Alcance exacto

Solo se modificó `Taller.Infrastructure/TallerDbContextFactory.cs` y se creó este archivo `entorno.md` en la raíz de la solución.

La clase implementa `IDesignTimeDbContextFactory<TallerDbContext>`: EF la utiliza para operaciones de diseño, como las migraciones. **El arranque normal de la API sigue utilizando la configuración de conexión de `Program.cs`**, que no se modificó por el alcance solicitado. Por tanto, estas cuatro variables centralizan la conexión de esta fábrica, pero todavía no sustituyen la configuración de conexión de la API en ejecución. El método público `BuildConnectionString` queda disponible para reutilizarlo si posteriormente se autoriza integrar ese registro.
