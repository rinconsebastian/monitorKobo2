using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App_consulta.Migrations
{
    public partial class InitialCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDDependencia",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Policy",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false),
                    claim = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Responsable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdJefe = table.Column<int>(type: "int", nullable: true),
                    Nombre = table.Column<string>(type: "longtext", nullable: false),
                    Editar = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responsable_Responsable_IdJefe",
                        column: x => x.IdJefe,
                        principalTable: "Responsable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1", "97f6ff5b-6816-44fc-8e6f-bbdedd1223f9", "Administrador", "ADMINISTRADOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Apellido", "ConcurrencyStamp", "Email", "EmailConfirmed", "IDDependencia", "LockoutEnabled", "LockoutEnd", "Nombre", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "", "05622443-5cfd-4389-8879-4523ac4c5aee", "admin@admin.com", true, 1, false, null, "Admin", "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAECPDxHYYnrFlyL6ghv6NFqs7g9ZlRCuHRIgzChzRa5GDZpnwsj563VfwncgzZt+OTw==", null, false, "NNK44MKHKTBOV6DHXJ4BT2Q3SYO3WQC2", false, "admin@admin.com" });

            migrationBuilder.InsertData(
                table: "Policy",
                columns: new[] { "id", "claim", "nombre" },
                values: new object[,]
                {
                    { 14, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", "Ver registro actividad" },
                    { 13, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", "Editar usuarios" },
                    { 12, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", "Editar roles" },
                    { 11, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Evaluacion.Editar", "Editar evaluaciones" },
                    { 10, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Campo.Editar", "Editar campos" },
                    { 9, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nivel.Editar", "Editar niveles" },
                    { 8, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", "Editar dependencias" },
                    { 7, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Categoria.Editar", "Editar categorias" },
                    { 6, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Periodo.Editar", "Editar periodo" },
                    { 5, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Indicador.Editar", "Editar indicadores" },
                    { 4, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Planeacion.Editar", "Editar planeación" },
                    { 3, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Ejecucion.Editar", "Editar ejecución" },
                    { 2, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Responsable", "Configuración dependencia" },
                    { 1, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.General", "Ver Configuración general" },
                    { 15, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nota.Editar", "Editar notas" }
                });

            migrationBuilder.InsertData(
                table: "Responsable",
                columns: new[] { "Id", "Editar", "IdJefe", "Nombre" },
                values: new object[] { 1, true, null, "Entidad" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.General", "1", "1" },
                    { 2, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Responsable", "1", "1" },
                    { 3, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Ejecucion.Editar", "1", "1" },
                    { 4, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Planeacion.Editar", "1", "1" },
                    { 5, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Indicador.Editar", "1", "1" },
                    { 6, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Periodo.Editar", "1", "1" },
                    { 7, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Categoria.Editar", "1", "1" },
                    { 8, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", "1", "1" },
                    { 9, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nivel.Editar", "1", "1" },
                    { 10, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Campo.Editar", "1", "1" },
                    { 11, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Evaluacion.Editar", "1", "1" },
                    { 12, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", "1", "1" },
                    { 13, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", "1", "1" },
                    { 14, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", "1", "1" },
                    { 15, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Nota.Editar", "1", "1" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_IdJefe",
                table: "Responsable",
                column: "IdJefe");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Policy");

            migrationBuilder.DropTable(
                name: "Responsable");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

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
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IDDependencia",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256);
        }
    }
}
