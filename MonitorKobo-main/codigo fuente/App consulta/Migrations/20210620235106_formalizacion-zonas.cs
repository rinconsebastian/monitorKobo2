using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class formalizacionzonas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "FormalizationConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 1,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 2,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 3,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 4,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 5,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 6,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 7,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 8,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 10,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 11,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 12,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 13,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 14,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 15,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 16,
                column: "Group",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 17,
                column: "Group",
                value: 1);

            migrationBuilder.InsertData(
                table: "FormalizationConfig",
                columns: new[] { "Id", "Field", "Group", "Name", "Value" },
                values: new object[,]
                {
                    { 20, "zonaOtro", 2, "Zona pesca (Otro)", "_id" },
                    { 19, "zonaNombre", 2, "Zona pesca (Nombre)", "_id" },
                    { 18, "zonaTipo", 2, "Zona pesca (Tipo)", "_id" }
                });

            migrationBuilder.InsertData(
                table: "FormalizationVariable",
                columns: new[] { "Id", "Group", "Key", "Value" },
                values: new object[] { 24, "Arte", "12", "Otro" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "FormalizationVariable",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DropColumn(
                name: "Group",
                table: "FormalizationConfig");
        }
    }
}
