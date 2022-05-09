using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class correccionpolicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "group",
                table: "Policy",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 1,
                column: "group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 2,
                column: "group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 3,
                column: "group",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 4,
                column: "group",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 5,
                column: "group",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 6,
                column: "group",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 7,
                column: "group",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 8,
                column: "group",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 9,
                column: "group",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "claim", "group" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Actualizar", 4 });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "claim", "group", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Listado", 4, "Informe encuestas" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 12,
                column: "group",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 13,
                column: "group",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 14,
                column: "group",
                value: 5);

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "group", "nombre" },
                values: new object[,]
                {
                    { 16, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Usuario", 4, "Ver encuestas por usuario" },
                    { 15, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Listado", 5, "Informe formalización" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DropColumn(
                name: "group",
                table: "Policy");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 10,
                column: "claim",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Kobo.Actualizar");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Informes.Encuestadores", "Ver Informe Encuestadores" });
        }
    }
}
