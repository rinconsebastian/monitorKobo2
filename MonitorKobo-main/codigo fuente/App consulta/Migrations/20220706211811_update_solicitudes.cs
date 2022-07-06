using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class update_solicitudes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertEmail",
                table: "RequestUser",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "RequestUser",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordProject",
                table: "RequestUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertEmail",
                table: "RequestUser");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "RequestUser");

            migrationBuilder.DropColumn(
                name: "RecordProject",
                table: "RequestUser");
        }
    }
}
