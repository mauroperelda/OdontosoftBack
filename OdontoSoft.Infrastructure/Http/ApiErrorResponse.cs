using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OdontoSoft.Infrastructure.Http
{
  /// <summary>
  /// Helpers para construir respuestas de error en el formato uniforme
  /// que el front entiende:
  /// <code>
  /// { "error": "Mensaje general", "errors": { "Campo": ["..."] } }
  /// </code>
  ///
  /// Usar SIEMPRE este helper desde los controllers (en vez de
  /// <c>BadRequest(new { error = ... })</c> ad-hoc) garantiza que el
  /// contrato con el front sea consistente.
  /// </summary>
  public static class ApiErrorResponse
  {
    public static BadRequestObjectResult BadRequest(
        string message,
        IDictionary<string, string[]>? fieldErrors = null)
    {
      return new BadRequestObjectResult(new
      {
        error = message,
        errors = fieldErrors ?? new Dictionary<string, string[]>()
      });
    }

    public static NotFoundObjectResult NotFound(string message)
    {
      return new NotFoundObjectResult(new
      {
        error = message,
        errors = new Dictionary<string, string[]>()
      });
    }

    public static ConflictObjectResult Conflict(
        string message,
        IDictionary<string, string[]>? fieldErrors = null)
    {
      return new ConflictObjectResult(new
      {
        error = message,
        errors = fieldErrors ?? new Dictionary<string, string[]>()
      });
    }

    // â”€â”€ TraducciÃ³n de excepciones comunes â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>
    /// Traduce una excepciÃ³n capturada en un controller a un IActionResult
    /// con mensaje legible. Cubre las excepciones mÃ¡s comunes de EF Core /
    /// PostgreSQL que el usuario no deberÃ­a ver crudas.
    /// </summary>
    public static IActionResult FromException(Exception ex, string? entityName = null)
    {
      var dbEx = FindDbUpdateException(ex);
      if (dbEx is not null)
      {
        return FromDbUpdateException(dbEx, entityName);
      }

      // InvalidOperationException suele ser un problema de lÃ³gica de negocio
      // (ej. "no se puede eliminar porque tiene turnos asociados").
      if (ex is InvalidOperationException)
      {
        return BadRequest(ex.Message);
      }

      // Fallback: no exponer el mensaje crudo si no es ninguno de los
      // esperados. El logger deberÃ­a haberlo capturado antes.
      return BadRequest("OcurriÃ³ un error al procesar la solicitud.");
    }

    private static DbUpdateException? FindDbUpdateException(Exception? ex)
    {
      while (ex is not null)
      {
        if (ex is DbUpdateException dbu) return dbu;
        ex = ex.InnerException;
      }
      return null;
    }

    private static IActionResult FromDbUpdateException(DbUpdateException ex, string? entityName)
    {
      // PostgreSQL expone PostgresException con SQLSTATE codes:
      //  23505 â†’ unique violation (DNI duplicado, etc.)
      //  23503 â†’ foreign key violation (no se puede borrar, tiene FKs)
      //  23502 â†’ not null violation
      //  23514 â†’ check violation
      var pg = FindPostgresException(ex);
      if (pg is null)
      {
        return BadRequest("No se pudo guardar los datos. RevisÃ¡ que no falte informaciÃ³n obligatoria.");
      }

      switch (pg.SqlState)
      {
        case "23505":
          {
            // Extraer el nombre del Ã­ndice para inferir el campo.
            // El mensaje viene como: 'duplicate key value violates unique constraint "IX_paciente_dni"'
            // DespuÃ©s del snake_case: "IX_paciente_dni" â†’ "Dni" (Ãºltimo segmento, PascalCase)
            var field = ExtractFieldFromUniqueConstraint(pg, entityName);
            var msg = field is null
                ? "Ya existe un registro con esos datos."
                : $"Ya existe un registro con ese {field}.";
            return field is null
                ? BadRequest(msg)
                : BadRequest(msg, new Dictionary<string, string[]>
                {
                  [field] = new[] { "Este valor ya estÃ¡ registrado." }
                });
          }
        case "23503":
          {
            // Foreign key: la fila tiene dependencias (turnos, pagos, etc.)
            // o referencia una fila inexistente. PG no distingue tan
            // limpio como SQL Server entre insert/update vs delete, asÃ­
            // que mostramos un mensaje genÃ©rico que cubre ambos casos.
            return BadRequest(
                "No se puede completar la operaciÃ³n porque el registro tiene datos asociados "
                + "o referencia datos inexistentes.");
          }
        case "23502":
          {
            // null value in column "x" of relation "y" violates not-null constraint
            var field = ExtractFieldFromNotNull(pg);
            var msg = field is null
                ? "Faltan datos obligatorios."
                : $"El campo {field} es obligatorio.";
            return field is null
                ? BadRequest(msg)
                : BadRequest(msg, new Dictionary<string, string[]>
                {
                  [field] = new[] { "Este campo es obligatorio." }
                });
          }
        case "23514":
          return BadRequest("Los datos no cumplen las validaciones de la base de datos.");
        default:
          return BadRequest("No se pudo guardar los datos. RevisÃ¡ la informaciÃ³n enviada.");
      }
    }

    private static PostgresException? FindPostgresException(Exception? ex)
    {
      while (ex is not null)
      {
        if (ex is PostgresException pg) return pg;
        ex = ex.InnerException;
      }
      return null;
    }

    // "duplicate key value violates unique constraint \"IX_paciente_dni\""
    //  â†’ constraint name "IX_paciente_dni" â†’ field "Dni" (PascalCase)
    private static string? ExtractFieldFromUniqueConstraint(PostgresException pg, string? entityName)
    {
      // PostgresException.ConstraintName es la fuente mÃ¡s confiable cuando estÃ¡
      // disponible (Npgsql 6+ lo popula). Si no, parseamos el mensaje.
      var constraintName = pg.ConstraintName;
      if (string.IsNullOrEmpty(constraintName))
      {
        var match = Regex.Match(
            pg.MessageText,
            @"unique constraint\s+""([^""]+)""",
            RegexOptions.IgnoreCase);
        if (match.Success) constraintName = match.Groups[1].Value;
      }
      if (string.IsNullOrEmpty(constraintName)) return null;

      // ConvenciÃ³n de nombres: "IX_<tabla>_<columna>" o "UQ_<tabla>_<columna>".
      // DespuÃ©s de snake_case: "IX_paciente_dni" â†’ Ãºltima parte = "dni" â†’ "Dni".
      var lastUnderscore = constraintName.LastIndexOf('_');
      if (lastUnderscore < 0 || lastUnderscore == constraintName.Length - 1) return null;
      var tail = constraintName[(lastUnderscore + 1)..];

      return SnakeToPascal(tail);
    }

    // "null value in column \"nombre\" of relation \"paciente\" violates not-null constraint"
    //  â†’ column "nombre" â†’ field "Nombre"
    private static string? ExtractFieldFromNotNull(PostgresException pg)
    {
      var columnName = pg.ColumnName;
      if (string.IsNullOrEmpty(columnName))
      {
        var match = Regex.Match(
            pg.MessageText,
            @"column\s+""([^""]+)""",
            RegexOptions.IgnoreCase);
        if (match.Success) columnName = match.Groups[1].Value;
      }
      if (string.IsNullOrEmpty(columnName)) return null;
      return SnakeToPascal(columnName);
    }

    // "dni" â†’ "Dni", "created_user_id" â†’ "CreatedUserId"
    // El front espera PascalCase para matchear los nombres de las propiedades del C# model.
    private static string SnakeToPascal(string snake)
    {
      var parts = snake.Split('_', StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < parts.Length; i++)
      {
        if (parts[i].Length == 0) continue;
        parts[i] = char.ToUpperInvariant(parts[i][0]) + parts[i][1..];
      }
      return string.Concat(parts);
    }
  }
}
