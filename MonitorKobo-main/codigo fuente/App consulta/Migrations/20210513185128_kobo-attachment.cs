using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class koboattachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KoboAttachment",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KoboUsername",
                table: "Configuracion",
                type: "longtext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KoboAttachment",
                table: "Configuracion");

            migrationBuilder.DropColumn(
                name: "KoboUsername",
                table: "Configuracion");
        }
    }
}
