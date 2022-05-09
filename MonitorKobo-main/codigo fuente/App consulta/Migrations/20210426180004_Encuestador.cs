using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class Encuestador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.AddColumn<int>(
                name: "CodeEncuestador",
                table: "Configuracion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KoboApiToken",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KoboAssetUid",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KoboKpiUrl",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KoboParamsMap",
                table: "Configuracion",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdParent = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    IdLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Location_IdParent",
                        column: x => x.IdParent,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_LocationLevel_IdLevel",
                        column: x => x.IdLevel,
                        principalTable: "LocationLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pollster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    Email = table.Column<string>(type: "longtext", nullable: true),
                    DNI = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: true),
                    IdLocation = table.Column<int>(type: "int", nullable: false),
                    IdResponsable = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pollster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pollster_AspNetUsers_IdUser",
                        column: x => x.IdUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pollster_Location_IdLocation",
                        column: x => x.IdLocation,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pollster_Responsable_IdResponsable",
                        column: x => x.IdResponsable,
                        principalTable: "Responsable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Ver");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Administrar");

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", "Editar dependencias" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", "Editar roles" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", "Editar usuarios" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", "Ver registro actividad" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Editar", "Editar encuestador" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Ver", "Ver encuestador" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Administrar", "Administrar encuestador" });

            migrationBuilder.CreateIndex(
                name: "IX_Location_IdLevel",
                table: "Location",
                column: "IdLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Location_IdParent",
                table: "Location",
                column: "IdParent");

            migrationBuilder.CreateIndex(
                name: "IX_Pollster_DNI",
                table: "Pollster",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pollster_IdLocation",
                table: "Pollster",
                column: "IdLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Pollster_IdResponsable",
                table: "Pollster",
                column: "IdResponsable");

            migrationBuilder.CreateIndex(
                name: "IX_Pollster_IdUser",
                table: "Pollster",
                column: "IdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pollster");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "LocationLevel");

            migrationBuilder.DropColumn(
                name: "CodeEncuestador",
                table: "Configuracion");

            migrationBuilder.DropColumn(
                name: "KoboApiToken",
                table: "Configuracion");

            migrationBuilder.DropColumn(
                name: "KoboAssetUid",
                table: "Configuracion");

            migrationBuilder.DropColumn(
                name: "KoboKpiUrl",
                table: "Configuracion");

            migrationBuilder.DropColumn(
                name: "KoboParamsMap",
                table: "Configuracion");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Ejecucion.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Planeacion.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Indicador.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Periodo.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Categoria.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimType",
                value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nivel.Editar");

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 10, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Campo.Editar", "1", "1" },
                    { 11, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Evaluacion.Editar", "1", "1" },
                    { 12, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", "1", "1" },
                    { 13, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", "1", "1" },
                    { 14, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", "1", "1" },
                    { 15, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nota.Editar", "1", "1" }
                });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Ejecucion.Editar", "Editar ejecución" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Planeacion.Editar", "Editar planeación" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Indicador.Editar", "Editar indicadores" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Periodo.Editar", "Editar periodo" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Categoria.Editar", "Editar categorias" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", "Editar dependencias" });

            migrationBuilder.UpdateData(
                table: "Policy",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "claim", "nombre" },
                values: new object[] { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nivel.Editar", "Editar niveles" });

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "nombre" },
                values: new object[,]
                {
                    { 15, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nota.Editar", "Editar notas" },
                    { 14, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", "Ver registro actividad" },
                    { 13, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", "Editar usuarios" },
                    { 12, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", "Editar roles" },
                    { 11, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Evaluacion.Editar", "Editar evaluaciones" },
                    { 10, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Campo.Editar", "Editar campos" }
                });
        }
    }
}
