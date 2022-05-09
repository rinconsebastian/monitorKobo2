using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class formalizacionimprimir : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "group", "nombre" },
                values: new object[] { 17, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imprimir", 5, "Imprimir formalización" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 17);
        }
    }
}
