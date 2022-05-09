using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class formalizacionmetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Formalization",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IdCreateByUser",
                table: "Formalization",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdLastEditByUser",
                table: "Formalization",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdResponsable",
                table: "Formalization",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditDate",
                table: "Formalization",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 11,
                column: "nombre",
                value: "Informe encuestas");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 15,
                column: "nombre",
                value: "Informe formalización");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 16,
                column: "nombre",
                value: "Ver encuestas por usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Formalization_IdCreateByUser",
                table: "Formalization",
                column: "IdCreateByUser");

            migrationBuilder.CreateIndex(
                name: "IX_Formalization_IdLastEditByUser",
                table: "Formalization",
                column: "IdLastEditByUser");

            migrationBuilder.CreateIndex(
                name: "IX_Formalization_IdResponsable",
                table: "Formalization",
                column: "IdResponsable");

            migrationBuilder.AddForeignKey(
                name: "FK_Formalization_AspNetUsers_IdCreateByUser",
                table: "Formalization",
                column: "IdCreateByUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Formalization_AspNetUsers_IdLastEditByUser",
                table: "Formalization",
                column: "IdLastEditByUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Formalization_Responsable_IdResponsable",
                table: "Formalization",
                column: "IdResponsable",
                principalTable: "Responsable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formalization_AspNetUsers_IdCreateByUser",
                table: "Formalization");

            migrationBuilder.DropForeignKey(
                name: "FK_Formalization_AspNetUsers_IdLastEditByUser",
                table: "Formalization");

            migrationBuilder.DropForeignKey(
                name: "FK_Formalization_Responsable_IdResponsable",
                table: "Formalization");

            migrationBuilder.DropIndex(
                name: "IX_Formalization_IdCreateByUser",
                table: "Formalization");

            migrationBuilder.DropIndex(
                name: "IX_Formalization_IdLastEditByUser",
                table: "Formalization");

            migrationBuilder.DropIndex(
                name: "IX_Formalization_IdResponsable",
                table: "Formalization");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Formalization");

            migrationBuilder.DropColumn(
                name: "IdCreateByUser",
                table: "Formalization");

            migrationBuilder.DropColumn(
                name: "IdLastEditByUser",
                table: "Formalization");

            migrationBuilder.DropColumn(
                name: "IdResponsable",
                table: "Formalization");

            migrationBuilder.DropColumn(
                name: "LastEditDate",
                table: "Formalization");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 11,
                column: "nombre",
                value: "Informe Encuestas");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 15,
                column: "nombre",
                value: "Informe Formalización");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 16,
                column: "nombre",
                value: "Ver Encuestas por usuario");
        }
    }
}
