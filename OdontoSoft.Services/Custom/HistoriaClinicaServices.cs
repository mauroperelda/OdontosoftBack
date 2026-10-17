using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;
using Odontosoft.Services;

namespace OdontoSoft.Services;

/// <summary>
/// Implementación de IHistoriaClinicaServices. Extrae la lógica que vivía
/// inline en HistoriaClinicaController (crear HC, upsert de piezas, fichas).
/// </summary>
public partial class HistoriaClinicaServices
{
  public UpsertResult Create(HistoriaClinicaDto vm)
  {
    var existente = _unitOfWork.HistoriaClinicaRepository.GetByPacienteId(vm.PacienteId);
    if (existente != null)
    {
      return UpsertResult.Fail(
          "El paciente ya tiene una historia clínica creada.",
          new Dictionary<string, string>
          {
            ["PacienteId"] = "Este paciente ya tiene historia clínica."
          });
    }

    var hc = new HistoriaClinica();
    vm.MapDtoToHistoriaClinica(hc);
    _unitOfWork.HistoriaClinicaRepository.Add(hc);
    _unitOfWork.Save();
    return UpsertResult.Created(hc.Id);
  }

  public UpsertResult Update(int id, HistoriaClinicaDto vm)
  {
    var hc = _unitOfWork.HistoriaClinicaRepository.GetById(id);
    if (hc == null) return UpsertResult.Fail("Historia clínica no encontrada.");

    vm.Id = id;
    vm.MapDtoToHistoriaClinica(hc);
    _unitOfWork.HistoriaClinicaRepository.Upsert(hc);
    _unitOfWork.Save();
    return UpsertResult.Created(hc.Id);
  }

  public ServiceResult GuardarPieza(OdontogramaPiezaDto vm)
  {
    var pieza = _unitOfWork.OdontogramaPiezaRepository
        .GetByHistoriaClinicaIdAndNumeroPieza(vm.HistoriaClinicaId, vm.NumeroPieza);

    if (pieza != null)
    {
      vm.Id = pieza.Id;
      vm.MapDtoToOdontogramaPieza(pieza);
      _unitOfWork.OdontogramaPiezaRepository.Upsert(pieza);
    }
    else
    {
      pieza = new OdontogramaPieza();
      vm.MapDtoToOdontogramaPieza(pieza);
      _unitOfWork.OdontogramaPiezaRepository.Add(pieza);
    }

    _unitOfWork.Save();
    return ServiceResult.Ok();
  }

  public UpsertResult GuardarFicha(FichaTratamientoDto vm)
  {
    FichaTratamiento ficha;
    if (vm.Id != 0)
    {
      ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(vm.Id);
      if (ficha == null) return UpsertResult.Fail("Ficha de tratamiento no encontrada.");
    }
    else
    {
      ficha = new FichaTratamiento();
      ficha.FechaRealizacion = DateTime.Now;
    }

    vm.MapDtoToFichaTratamiento(ficha);

    if (vm.Id != 0)
      _unitOfWork.FichaTratamientoRepository.Upsert(ficha);
    else
      _unitOfWork.FichaTratamientoRepository.Add(ficha);

    _unitOfWork.Save();
    return UpsertResult.Created(ficha.Id);
  }

  public ServiceResult EliminarFicha(int fichaId)
  {
    _unitOfWork.FichaTratamientoRepository.Delete(fichaId);
    _unitOfWork.Save();
    return ServiceResult.Ok();
  }

  public IEnumerable<(int Id, string Nombre)> GetTratamientosParaFicha()
  {
    return _unitOfWork.TratamientoRepository
        .GetAllEnabled()
        .Select(t => new { t.Id, t.Nombre })
        .ToList()
        .Select(x => (x.Id, x.Nombre));
  }
}
