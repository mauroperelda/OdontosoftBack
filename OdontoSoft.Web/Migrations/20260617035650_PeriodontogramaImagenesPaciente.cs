using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Web.Migrations
{
    /// <inheritdoc />
    public partial class PeriodontogramaImagenesPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTA: el modelo tiene drift preexistente (Turno.UbicacionId,
            // Presupuesto.ProfesionalId, Ubicacion, TratamientoPrecio) que ya
            // está aplicado en la DB por scripts manuales. NO los tocamos acá
            // para no duplicar columnas. El drift se resolverá en una migración
            // aparte cuando se sincronicen DB ↔ modelo.
            //
            // Esta migración solo agrega lo propio de la ficha del paciente:
            //   - Periodontograma (cabecera de evaluación periodontal)
            //   - PeriodontogramaPieza (32 piezas por evaluación)
            //   - ImagenPaciente (radiografías, fotos, documentos)

            migrationBuilder.CreateTable(
                name: "Periodontograma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoriaClinicaId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Periodontograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Periodontograma_HistoriaClinica_HistoriaClinicaId",
                        column: x => x.HistoriaClinicaId,
                        principalTable: "HistoriaClinica",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodontogramaPieza",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodontogramaId = table.Column<int>(type: "int", nullable: false),
                    NumeroPieza = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sangrado = table.Column<bool>(type: "bit", nullable: false),
                    Supuracion = table.Column<bool>(type: "bit", nullable: false),
                    Placa = table.Column<bool>(type: "bit", nullable: false),
                    BolsaPeriodontal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Recesion = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PeriodontogramaPieza", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodontogramaPieza_Periodontograma_PeriodontogramaId",
                        column: x => x.PeriodontogramaId,
                        principalTable: "Periodontograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImagenPaciente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoriaClinicaId = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_ImagenPaciente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagenPaciente_HistoriaClinica_HistoriaClinicaId",
                        column: x => x.HistoriaClinicaId,
                        principalTable: "HistoriaClinica",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Periodontograma_HistoriaClinicaId",
                table: "Periodontograma",
                column: "HistoriaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodontogramaPieza_PeriodontogramaId",
                table: "PeriodontogramaPieza",
                column: "PeriodontogramaId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenPaciente_HistoriaClinicaId",
                table: "ImagenPaciente",
                column: "HistoriaClinicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ImagenPaciente");
            migrationBuilder.DropTable(name: "PeriodontogramaPieza");
            migrationBuilder.DropTable(name: "Periodontograma");
        }
    }
}
