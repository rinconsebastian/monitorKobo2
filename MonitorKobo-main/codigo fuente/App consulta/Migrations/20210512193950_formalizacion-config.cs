using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class formalizacionconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FormalizationConfig",
                columns: new[] { "Id", "Field", "Name", "Value" },
                values: new object[,]
                {
                    { 1, "IdKobo", "Id Kobo", "_id" },
                    { 2, "NumeroRegistro", "Número registro", "_id" },
                    { 3, "Name", "Nombre y apellidos", "" },
                    { 4, "FechaSolicitud", "Fecha solicitud", "_id" },
                    { 5, "Cedula", "Cédula pescador", "_id" },
                    { 6, "Departamento", "Departamento", "_id" },
                    { 7, "Municipio", "Municipio", "_id" },
                    { 8, "AreaPesca", "Área de pesca", "_id" },
                    { 9, "ArtesPesca", "Artes de pesca", "_id" },
                    { 10, "NombreAsociacion", "Nombre asociación", "_id" },
                    { 11, "ImgPescador", "Foto pescador", "_id" },
                    { 12, "ImgSolicitudCarnet", "Foto solicitud carnet", "_id" },
                    { 13, "ImgCertificacion", "Foto certificación", "_id" },
                    { 14, "ImgCedulaAnverso", "Foto cédula (anverso)", "_id" },
                    { 15, "ImgCedulaReverso", "Foto cédula (reverso)", "_id" },
                    { 16, "ImgFirmaDigital", "Firma digital", "_id" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "FormalizationConfig",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
