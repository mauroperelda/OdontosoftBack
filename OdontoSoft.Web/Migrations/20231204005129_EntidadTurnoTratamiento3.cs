using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    public partial class EntidadTurnoTratamiento3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tratamiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tratamiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pacienteId = table.Column<int>(type: "int", nullable: false),
                    ProfesionalId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TratamientoId = table.Column<int>(type: "int", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turno_Paciente_pacienteId",
                        column: x => x.pacienteId,
                        principalTable: "Paciente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turno_Profesional_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "Profesional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turno_Tratamiento_TratamientoId",
                        column: x => x.TratamientoId,
                        principalTable: "Tratamiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turno_pacienteId",
                table: "Turno",
                column: "pacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_ProfesionalId",
                table: "Turno",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_TratamientoId",
                table: "Turno",
                column: "TratamientoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turno");

            migrationBuilder.DropTable(
                name: "Tratamiento");
        }
    }
}
