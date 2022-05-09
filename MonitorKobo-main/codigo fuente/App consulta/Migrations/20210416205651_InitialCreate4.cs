using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class InitialCreate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuracion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Logo = table.Column<string>(type: "longtext", nullable: true),
                    ImgHeader = table.Column<string>(type: "longtext", nullable: true),
                    ImgBackgroud = table.Column<string>(type: "longtext", nullable: true),
                    colorTextoHeader = table.Column<string>(type: "longtext", nullable: true),
                    colorPrincipal = table.Column<string>(type: "longtext", nullable: true),
                    colorTextoPrincipal = table.Column<string>(type: "longtext", nullable: true),
                    contacto = table.Column<string>(type: "longtext", nullable: false),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Entidad = table.Column<string>(type: "longtext", nullable: false),
                    NombrePlan = table.Column<string>(type: "longtext", nullable: false),
                    libre = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracion", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "Configuracion",
                columns: new[] { "id", "Entidad", "ImgBackgroud", "ImgHeader", "Logo", "NombrePlan", "activo", "colorPrincipal", "colorTextoHeader", "colorTextoPrincipal", "contacto", "libre" },
                values: new object[] { 1, "Entidad", null, null, "/images/SIE.png", "Plan", true, "#52a3a1", "#ffffff", "#00000", "rinconsebastian@gmail.com", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuracion");
        }
    }
}
