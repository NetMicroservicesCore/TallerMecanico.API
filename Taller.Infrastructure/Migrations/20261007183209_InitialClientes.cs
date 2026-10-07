using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taller.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreadoUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    NombreCompletoContacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailContacto = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Colonia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Municipio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Nombre_Id",
                table: "Clientes",
                columns: new[] { "Nombre", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
