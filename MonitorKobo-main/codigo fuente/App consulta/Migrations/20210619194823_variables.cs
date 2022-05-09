using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class variables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormalizationVariable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Group = table.Column<string>(type: "longtext", nullable: false),
                    Key = table.Column<string>(type: "longtext", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormalizationVariable", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FormalizationVariable",
                columns: new[] { "Id", "Group", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "Zona", "1", "Arroyo" },
                    { 22, "Arte", "10", "Trampas/nasas" },
                    { 21, "Arte", "9", "Redes de enmalle" },
                    { 20, "Arte", "8", "Palangre" },
                    { 19, "Arte", "7", "Línea de mano" },
                    { 18, "Arte", "6", "Cóngolo / canasta" },
                    { 17, "Arte", "5", "Chinchorro" },
                    { 16, "Arte", "4", "Chinchorra" },
                    { 15, "Arte", "3", "Boliche" },
                    { 14, "Arte", "2", "Atarraya" },
                    { 13, "Arte", "1", "Arpón" },
                    { 12, "Zona", "14", "Embalse" },
                    { 11, "Zona", "11", "Sector de río" },
                    { 10, "Zona", "10", "Río" },
                    { 9, "Zona", "9", "Riachuelo" },
                    { 8, "Zona", "8", "Quebrada" },
                    { 7, "Zona", "7", "Presa" },
                    { 6, "Zona", "6", "Lago" },
                    { 5, "Zona", "5", "Laguna" },
                    { 4, "Zona", "4", "Estanque" },
                    { 3, "Zona", "3", "Ciénaga" },
                    { 2, "Zona", "2", "Canal" },
                    { 23, "Arte", "11", "Trasmallo" }
                });

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "group", "nombre" },
                values: new object[] { 18, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Exportar.Listado", 6, "Exportar listados" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormalizationVariable");

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 18);
        }
    }
}
