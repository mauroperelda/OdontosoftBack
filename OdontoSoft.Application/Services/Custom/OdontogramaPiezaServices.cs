using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Common;

using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services;

/// <summary>
/// Implementación de IOdontogramaPiezaServices. La regla de upsert por
/// (HC, NumeroPieza) es la misma que usa HistoriaClinicaController.GuardarPieza;
/// al centralizarla acá evitamos la divergencia.
/// </summary>
public partial class OdontogramaPiezaServices
{
  public ServiceResult UpsertByPieza(OdontogramaPiezaDto vm)
  {
    var existing = _unitOfWork.OdontogramaPiezaRepository
        .GetByHistoriaClinicaIdAndNumeroPieza(vm.HistoriaClinicaId, vm.NumeroPieza);

    if (existing != null)
    {
      vm.Id = existing.Id;
      vm.MapDtoToOdontogramaPieza(existing);
      _unitOfWork.OdontogramaPiezaRepository.Upsert(existing);
      _unitOfWork.Save();
      return ServiceResult.Ok();
    }

    var pieza = new OdontogramaPieza();
    vm.MapDtoToOdontogramaPieza(pieza);
    _unitOfWork.OdontogramaPiezaRepository.Add(pieza);
    _unitOfWork.Save();
    return ServiceResult.Ok();
  }
}
