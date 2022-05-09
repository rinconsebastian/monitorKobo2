using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class asociacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Encuestador",
                table: "Formalization",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KoboAssetUidAssociation",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.InsertData(
                table: "FormalizationConfig",
                columns: new[] { "Id", "Field", "Name", "Value" },
                values: new object[] { 17, "Encuestador", "Cedula Encuestador", "_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 17);


            migrationBuilder.DropColumn(
                name: "Encuestador",
                table: "Formalization");

            migrationBuilder.DropColumn(
                name: "KoboAssetUidAssociation",
                table: "Configuracion");
        }
    }
}
