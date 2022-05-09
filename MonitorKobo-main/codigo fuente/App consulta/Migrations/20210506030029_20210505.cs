using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class _20210505 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Favicon",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "nombre" },
                values: new object[,]
                {
                    { 10, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Kobo.Actualizar", "Actualizar encuestas" },
                    { 11, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Informes.Encuestadores", "Ver Informe Encuestadores" },
                    { 12, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Ver", "Ver formalización" },
                    { 13, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Editar", "Editar formalización" },
                    { 14, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Validar", "Validar formalización" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DropColumn(
                name: "Favicon",
                table: "Configuracion");
        }
    }
}
