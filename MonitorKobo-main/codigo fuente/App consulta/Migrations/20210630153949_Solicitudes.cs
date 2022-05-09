using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class Solicitudes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Request = table.Column<string>(type: "longtext", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: false),
                    FormalizationId = table.Column<int>(type: "int", nullable: false),
                    File = table.Column<string>(type: "longtext", nullable: true),
                    Response = table.Column<string>(type: "longtext", nullable: true),
                    AlertUser = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AlertAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdUser = table.Column<string>(type: "longtext", nullable: true),
                    AdminName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestUser", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "group", "nombre" },
                values: new object[,]
                {
                    { 19, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imagen.Cambiar", 5, "Cambiar imagenes formalización" },
                    { 20, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imagen.Restablecer", 5, "Restablecer imagenes formalización" },
                    { 21, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Crear", 7, "Crear solicitudes" },
                    { 22, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Administrar", 7, "Administrar solicitudes" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestUser");

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 22);
        }
    }
}
