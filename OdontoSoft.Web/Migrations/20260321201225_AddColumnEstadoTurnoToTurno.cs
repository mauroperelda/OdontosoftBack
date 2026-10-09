using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnEstadoTurnoToTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoTurnoId",
                table: "Turno",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Turno_EstadoTurnoId",
                table: "Turno",
                column: "EstadoTurnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_EstadoTurno_EstadoTurnoId",
                table: "Turno",
                column: "EstadoTurnoId",
                principalTable: "EstadoTurno",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turno_EstadoTurno_EstadoTurnoId",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_Turno_EstadoTurnoId",
                table: "Turno");

            migrationBuilder.DropColumn(
                name: "EstadoTurnoId",
                table: "Turno");
        }
    }
}
