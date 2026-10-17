using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;
using Odontosoft.Services;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio de Turnos. Extraída de TurnoController.
/// Mantiene la validación de conflicto de horario que antes vivía en el
/// controller (líneas 88-97 de TurnoController.cs pre-Fase 4).
/// </summary>
public partial class TurnoServices
{
  public UpsertResult Create(TurnoDto vm)
  {
    if (vm.UbicacionId == 0) vm.UbicacionId = null;

    var conflict = ValidarConflictoHorario(vm.ProfesionalId, vm.HoraInicio ?? DateTime.MinValue);
    if (!conflict.IsSuccess) return UpsertResult.Fail(conflict.Error, conflict.FieldErrors);

    var turno = new Turno();
    vm.MapDtoToTurno(turno);
    _unitOfWork.TurnoRepository.Add(turno);
    _unitOfWork.Save();
    return UpsertResult.Created(turno.Id);
  }

  public UpsertResult Update(int id, TurnoDto vm)
  {
    if (vm.UbicacionId == 0) vm.UbicacionId = null;

    var turno = _unitOfWork.TurnoRepository.GetByTurnoId(id);
    if (turno == null) return UpsertResult.Fail("Turno no encontrado.");

    var conflict = ValidarConflictoHorario(vm.ProfesionalId, vm.HoraInicio ?? DateTime.MinValue, excludeTurnoId: id);
    if (!conflict.IsSuccess) return UpsertResult.Fail(conflict.Error, conflict.FieldErrors);

    vm.Id = id;
    vm.MapDtoToTurno(turno);
    _unitOfWork.TurnoRepository.Upsert(turno);
    _unitOfWork.Save();
    return UpsertResult.Created(turno.Id);
  }

  public ServiceResult ValidarConflictoHorario(int profesionalId, DateTime horaInicio, int? excludeTurnoId = null)
  {
    var existente = _unitOfWork.TurnoRepository.GetByProfesionalByHoraInicio(profesionalId, horaInicio);
    if (existente == null) return ServiceResult.Ok();
    if (excludeTurnoId.HasValue && existente.Id == excludeTurnoId.Value) return ServiceResult.Ok();
    return ServiceResult.Fail(
        $"El profesional ya tiene un turno a las {horaInicio:HH:mm}.",
        new Dictionary<string, string>
        {
          ["HoraInicio"] = $"El profesional ya tiene un turno a las {horaInicio:HH:mm}."
        });
  }
}
