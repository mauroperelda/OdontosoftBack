using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    /// <inheritdoc />
    public partial class TratamientoIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tratamiento pasa a ser opcional en Turno: Permitía que un turno
            // se agende sin tratamiento seleccionado (FK violation con TratamientoId=0).
            migrationBuilder.AlterColumn<int>(
                name: "TratamientoId",
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
                name: "TratamientoId",
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
