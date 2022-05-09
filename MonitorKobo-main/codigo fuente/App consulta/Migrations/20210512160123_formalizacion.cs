using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class formalizacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formalization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdKobo = table.Column<string>(type: "longtext", nullable: true),
                    NumeroRegistro = table.Column<string>(type: "longtext", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    FechaSolicitud = table.Column<string>(type: "longtext", nullable: false),
                    Cedula = table.Column<string>(type: "longtext", nullable: false),
                    Departamento = table.Column<string>(type: "longtext", nullable: true),
                    Municipio = table.Column<string>(type: "longtext", nullable: true),
                    AreaPesca = table.Column<string>(type: "longtext", nullable: true),
                    ArtesPesca = table.Column<string>(type: "longtext", nullable: true),
                    NombreAsociacion = table.Column<string>(type: "longtext", nullable: true),
                    ImgPescador = table.Column<string>(type: "longtext", nullable: true),
                    ImgSolicitudCarnet = table.Column<string>(type: "longtext", nullable: true),
                    ImgCertificacion = table.Column<string>(type: "longtext", nullable: true),
                    ImgCedulaAnverso = table.Column<string>(type: "longtext", nullable: true),
                    ImgCedulaReverso = table.Column<string>(type: "longtext", nullable: true),
                    ImgFirmaDigital = table.Column<string>(type: "longtext", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formalization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormalizationConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Field = table.Column<string>(type: "longtext", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormalizationConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Formalization");

            migrationBuilder.DropTable(
                name: "FormalizationConfig");
        }
    }
}
