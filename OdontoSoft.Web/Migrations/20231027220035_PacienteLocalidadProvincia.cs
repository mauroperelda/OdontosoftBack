using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    public partial class PacienteLocalidadProvincia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dni",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Apellido",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DomicilioCalle",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DomicilioNumero",
                table: "Paciente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Paciente",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LocalidadId",
                table: "Paciente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinciaId",
                table: "Paciente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinciaId = table.Column<int>(type: "int", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localidad_Provincia_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_LocalidadId",
                table: "Paciente",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_ProvinciaId",
                table: "Paciente",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_ProvinciaId",
                table: "Localidad",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_Localidad_LocalidadId",
                table: "Paciente",
                column: "LocalidadId",
                principalTable: "Localidad",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_Provincia_ProvinciaId",
                table: "Paciente",
                column: "ProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_Localidad_LocalidadId",
                table: "Paciente");

            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_Provincia_ProvinciaId",
                table: "Paciente");

            migrationBuilder.DropTable(
                name: "Localidad");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_LocalidadId",
                table: "Paciente");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_ProvinciaId",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "DomicilioCalle",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "DomicilioNumero",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "LocalidadId",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "Paciente");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Dni",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Apellido",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
