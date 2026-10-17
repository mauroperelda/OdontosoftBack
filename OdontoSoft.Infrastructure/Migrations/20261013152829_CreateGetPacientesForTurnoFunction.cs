using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontoSoft.Infrastructure.Migrations
{
  /// <summary>
  /// Reemplaza la stored procedure de SQL Server `SP_GetPacientesForTurno` por una
  /// PL/pgSQL function de PostgreSQL. La firma es la misma: recibe un string de
  /// bÃºsqueda y devuelve los pacientes activos cuyo nombre, apellido o DNI matcheen.
  ///
  /// Se invoca desde C# con `FromSqlRaw("SELECT * FROM get_pacientes_for_turno({0})", busqueda)`.
  /// </summary>
  public partial class CreateGetPacientesForTurnoFunction : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_pacientes_for_turno(busqueda TEXT)
                RETURNS TABLE(
                    id INT,
                    nombre TEXT,
                    apellido TEXT,
                    dni TEXT
                ) AS $$
                BEGIN
                    RETURN QUERY
                    SELECT p.id, p.nombre, p.apellido, p.dni
                    FROM paciente p
                    WHERE p.disabled = false
                      AND (
                          LOWER(p.nombre) LIKE '%' || LOWER(busqueda) || '%'
                          OR LOWER(p.apellido) LIKE '%' || LOWER(busqueda) || '%'
                          OR p.dni LIKE '%' || busqueda || '%'
                      )
                    ORDER BY p.apellido, p.nombre
                    LIMIT 50;
                END;
                $$ LANGUAGE plpgsql;
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_pacientes_for_turno(TEXT);");
    }
  }
}
