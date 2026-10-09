using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace OdontoSoft.API.Helpers
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

        // ── Traducción de excepciones comunes ─────────────────────────────

        /// <summary>
        /// Traduce una excepción capturada en un controller a un IActionResult
        /// con mensaje legible. Cubre las excepciones más comunes de EF Core /
        /// SQL Server que el usuario no debería ver crudas.
        /// </summary>
        public static IActionResult FromException(Exception ex, string? entityName = null)
        {
            var dbEx = FindDbUpdateException(ex);
            if (dbEx is not null)
            {
                return FromDbUpdateException(dbEx, entityName);
            }

            // InvalidOperationException suele ser un problema de lógica de negocio
            // (ej. "no se puede eliminar porque tiene turnos asociados").
            if (ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }

            // Fallback: no exponer el mensaje crudo si no es ninguno de los
            // esperados. El logger debería haberlo capturado antes.
            return BadRequest("Ocurrió un error al procesar la solicitud.");
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
            // SQL Server expone SqlException con códigos específicos:
            //  2627 / 2601 → unique constraint violation (DNI duplicado, etc.)
            //  547         → foreign key violation (no se puede borrar, tiene FKs)
            //  515         → cannot insert NULL
            //  50000       → RAISERROR desde un trigger
            var sql = FindSqlException(ex);
            if (sql is null)
            {
                return BadRequest("No se pudo guardar los datos. Revisá que no falte información obligatoria.");
            }

            switch (sql.Number)
            {
                case 2627:
                case 2601:
                {
                    // Extraer el nombre del índice para inferir el campo.
                    // El mensaje viene como: "Violation of UNIQUE KEY constraint 'IX_Paciente_Dni'..."
                    var field = ExtractFieldFromUniqueKeyMessage(sql.Message, entityName);
                    var msg = field is null
                        ? "Ya existe un registro con esos datos."
                        : $"Ya existe un registro con ese {field}.";
                    return field is null
                        ? BadRequest(msg)
                        : BadRequest(msg, new Dictionary<string, string[]>
                          {
                              [field] = new[] { "Este valor ya está registrado." }
                          });
                }
                case 547:
                {
                    // Foreign key: la fila tiene dependencias (turnos, pagos, etc.)
                    // o referencia una fila inexistente.
                    var isDeleteReference = sql.Message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase)
                                         || sql.Message.Contains("DELETE", StringComparison.OrdinalIgnoreCase);
                    if (isDeleteReference)
                    {
                        return BadRequest(
                            "No se puede eliminar el registro porque tiene datos asociados.");
                    }
                    return BadRequest(
                        "El registro referencia datos que no existen o ya fueron eliminados.");
                }
                case 515:
                {
                    // Cannot insert the value NULL into column 'X'
                    var field = ExtractFieldFromNullMessage(sql.Message);
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
                default:
                    return BadRequest("No se pudo guardar los datos. Revisá la información enviada.");
            }
        }

        private static SqlException? FindSqlException(Exception? ex)
        {
            while (ex is not null)
            {
                if (ex is SqlException sql) return sql;
                ex = ex.InnerException;
            }
            return null;
        }

        // "Violation of UNIQUE KEY constraint 'IX_Paciente_Dni'. ...
        //  The duplicate key value is (12345678)."
        private static string? ExtractFieldFromUniqueKeyMessage(string message, string? entityName)
        {
            // Heurística: tomar lo que venga después del último '_' en el
            // nombre del índice, pasarlo a camelCase para el front.
            // "IX_Paciente_Dni" → "Dni"
            // "UQ_Paciente_Dni" → "Dni"
            var match = System.Text.RegularExpressions.Regex.Match(
                message, @"'(IX|UQ|AK)_[^']+'", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!match.Success) return null;

            var indexName = match.Value.Trim('\'');
            var lastUnderscore = indexName.LastIndexOf('_');
            if (lastUnderscore < 0 || lastUnderscore == indexName.Length - 1) return null;
            var tail = indexName[(lastUnderscore + 1)..];

            // El front espera PascalCase (DomicilioNumero), devolvemos tal cual.
            return tail;
        }

        // "Cannot insert the value NULL into column 'DomicilioNumero', table '...'"
        private static string? ExtractFieldFromNullMessage(string message)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                message, @"column '(\w+)'", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
