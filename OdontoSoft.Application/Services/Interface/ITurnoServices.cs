using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Common;

namespace OdontoSoft.Application.Services;

/// <summary>
/// Lógica de negocio de Turnos. Extraída de TurnoController.
/// </summary>
public interface ITurnoServices
{
  /// <summary>
  /// Crea un turno. Valida que el profesional no tenga otro turno a la
  /// misma hora de inicio. Normaliza UbicacionId = 0 a null.
  /// </summary>
  UpsertResult Create(TurnoDto vm);

  /// <summary>
  /// Actualiza un turno. Al editar, excludeTurnoId evita que el turno
  /// actual aparezca como conflicto consigo mismo.
  /// </summary>
  UpsertResult Update(int id, TurnoDto vm);

  /// <summary>
  /// Valida que no haya conflicto de horario. Devuelve ServiceResult.Fail
  /// si el profesional ya tiene un turno a esa hora.
  /// </summary>
  ServiceResult ValidarConflictoHorario(int profesionalId, DateTime horaInicio, int? excludeTurnoId = null);
}
