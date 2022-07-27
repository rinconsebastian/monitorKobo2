using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class ftiv046 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AquacultureField",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    NameKobo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    NameDB = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    GroupMap = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    IdParent = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AquacultureField", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AquacultureVariable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Group = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AquacultureVariable", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Cancelar");

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "group", "nombre" },
                values: new object[] { 25, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Acuicultura.Imprimir", 8, "Imprimir acuicultura" });

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "group", "nombre" },
                values: new object[] { 24, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Acuicultura.Listado", 8, "Informe acuicultura" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AquacultureField");

            migrationBuilder.DropTable(
                name: "AquacultureVariable");

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imprimir");
        }
    }
}
