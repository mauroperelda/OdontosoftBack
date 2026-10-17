using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;
using Odontosoft.Services;

namespace OdontoSoft.Services;

/// <summary>
/// Implementación de IPeriodontogramaServices. El controller hoy hace todo
/// el armado de piezas inline — el service centraliza la regla "una
/// evaluación periodontograma agrupa 0..32 piezas (de 32 esperados)" y el
/// cálculo de métricas.
/// </summary>
public partial class PeriodontogramaServices
{
  public UpsertResult<PeriodontogramaDto> Create(PeriodontogramaDto vm)
  {
    if (vm.HistoriaClinicaId <= 0)
    {
      return UpsertResult<PeriodontogramaDto>.Fail(
          "Falta HistoriaClinicaId.",
          new Dictionary<string, string> { ["HistoriaClinicaId"] = "Falta HistoriaClinicaId." });
    }

    var entity = new Periodontograma
    {
      HistoriaClinicaId = vm.HistoriaClinicaId,
      FechaRegistro = vm.FechaRegistro == default ? DateTime.UtcNow : vm.FechaRegistro,
      Observaciones = vm.Observaciones,
    };
    _unitOfWork.PeriodontogramaRepository.Add(entity);
    _unitOfWork.Save();

    if (vm.Piezas != null)
    {
      foreach (var piezaVm in vm.Piezas)
      {
        _unitOfWork.PeriodontogramaRepository.AddPieza(new PeriodontogramaPieza
        {
          PeriodontogramaId = entity.Id,
          NumeroPieza = piezaVm.NumeroPieza,
          Estado = piezaVm.Estado,
          Sangrado = piezaVm.Sangrado,
          Supuracion = piezaVm.Supuracion,
          Placa = piezaVm.Placa,
          BolsaPeriodontal = piezaVm.BolsaPeriodontal,
          Recesion = piezaVm.Recesion,
          Observacion = piezaVm.Observacion,
        });
      }
      _unitOfWork.Save();
    }

    var loaded = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(entity.Id);
    var result = new PeriodontogramaDto(loaded);
    result.Metricas = CalcularMetricas(loaded.Piezas);
    return UpsertResult<PeriodontogramaDto>.Created(result);
  }

  public UpsertResult<PeriodontogramaDto> Update(int id, PeriodontogramaDto vm)
  {
    var entity = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
    if (entity == null) return UpsertResult<PeriodontogramaDto>.Fail("Periodontograma no encontrado.");

    entity.Observaciones = vm.Observaciones;
    if (vm.FechaRegistro != default) entity.FechaRegistro = vm.FechaRegistro;
    _unitOfWork.PeriodontogramaRepository.Upsert(entity);

    // Reemplazar piezas: borrar existentes, insertar nuevas.
    _unitOfWork.PeriodontogramaRepository.DeletePiezas(entity.Id);
    if (vm.Piezas != null)
    {
      foreach (var piezaVm in vm.Piezas)
      {
        _unitOfWork.PeriodontogramaRepository.AddPieza(new PeriodontogramaPieza
        {
          PeriodontogramaId = entity.Id,
          NumeroPieza = piezaVm.NumeroPieza,
          Estado = piezaVm.Estado,
          Sangrado = piezaVm.Sangrado,
          Supuracion = piezaVm.Supuracion,
          Placa = piezaVm.Placa,
          BolsaPeriodontal = piezaVm.BolsaPeriodontal,
          Recesion = piezaVm.Recesion,
          Observacion = piezaVm.Observacion,
        });
      }
    }
    _unitOfWork.Save();

    var loaded = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
    var result = new PeriodontogramaDto(loaded);
    result.Metricas = CalcularMetricas(loaded.Piezas);
    return UpsertResult<PeriodontogramaDto>.Created(result);
  }

  public ServiceResult Delete(int id)
  {
    var entity = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
    if (entity == null) return ServiceResult.Fail("Periodontograma no encontrado.");

    _unitOfWork.PeriodontogramaRepository.DeletePiezas(entity.Id);
    _unitOfWork.PeriodontogramaRepository.Delete(entity.Id);
    _unitOfWork.Save();
    return ServiceResult.Ok();
  }

  /// <summary>
  /// Métricas agregadas. Asume 32 piezas esperadas en un adulto (la
  /// convención del consultorio); si la colección está vacía, devuelve
  /// todo en 0 con 32 ausentes.
  /// </summary>
  public PeriodontogramaMetricasDto CalcularMetricas(ICollection<PeriodontogramaPieza> piezas)
  {
    const int totalEsperado = 32;
    if (piezas == null || piezas.Count == 0)
    {
      return new PeriodontogramaMetricasDto
      {
        BopPorcentaje = 0,
        PlacaPorcentaje = 0,
        DientesPresentes = 0,
        DientesAusentes = totalEsperado,
      };
    }

    int total = piezas.Count;
    int conSangrado = piezas.Count(p => p.Sangrado);
    int conPlaca = piezas.Count(p => p.Placa);

    return new PeriodontogramaMetricasDto
    {
      BopPorcentaje = total == 0 ? 0 : Math.Round((decimal)conSangrado * 100 / total, 2),
      PlacaPorcentaje = total == 0 ? 0 : Math.Round((decimal)conPlaca * 100 / total, 2),
      DientesPresentes = total,
      DientesAusentes = Math.Max(0, totalEsperado - total),
    };
  }
}
