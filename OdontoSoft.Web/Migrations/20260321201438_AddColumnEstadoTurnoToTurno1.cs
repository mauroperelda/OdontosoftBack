using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnEstadoTurnoToTurno1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstadoTurnoId",
                table: "Turno",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstadoTurnoId",
                table: "Turno",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
