using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    public partial class MigCambiosEntidadTurno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turno_Paciente_pacienteId",
                table: "Turno");

            migrationBuilder.RenameColumn(
                name: "pacienteId",
                table: "Turno",
                newName: "PacienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Turno_pacienteId",
                table: "Turno",
                newName: "IX_Turno_PacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_Paciente_PacienteId",
                table: "Turno",
                column: "PacienteId",
                principalTable: "Paciente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turno_Paciente_PacienteId",
                table: "Turno");

            migrationBuilder.RenameColumn(
                name: "PacienteId",
                table: "Turno",
                newName: "pacienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Turno_PacienteId",
                table: "Turno",
                newName: "IX_Turno_pacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_Paciente_pacienteId",
                table: "Turno",
                column: "pacienteId",
                principalTable: "Paciente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
