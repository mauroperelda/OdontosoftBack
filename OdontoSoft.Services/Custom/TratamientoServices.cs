using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;
using Odontosoft.Services;

namespace OdontoSoft.Services;

/// <summary>
/// Implementación de ITratamientoServices. La consulta "tratamientos con
/// precios por ubicación" se hace con un join in-memory sobre los All() de
/// TratamientoPrecio y Ubicacion — funciona bien con los volúmenes actuales
/// (decenas de tratamientos). Si crece, mover a un JOIN real en el repo.
/// </summary>
public partial class TratamientoServices
{
  public IEnumerable<TratamientoConPreciosDto> GetAllConPrecios()
  {
    var tratamientos = _unitOfWork.TratamientoRepository.GetAllEnabled().ToList();
    var ubicaciones = _unitOfWork.UbicacionRepository.GetAllEnabled().ToList();
    var precios = _unitOfWork.TratamientoPrecioRepository.All().ToList();

    return tratamientos.Select(t => new TratamientoConPreciosDto
    {
      Id = t.Id,
      Codigo = t.Codigo,
      Nombre = t.Nombre,
      Descripcion = t.Descripcion,
      Disabled = t.Disabled,
      Precios = precios
            .Where(p => p.TratamientoId == t.Id)
            .Select(p => new PrecioPorUbicacion
            {
              UbicacionId = p.UbicacionId,
              UbicacionNombre = ubicaciones.FirstOrDefault(u => u.Id == p.UbicacionId)?.Nombre ?? "",
              Precio = p.Precio,
            })
            .ToList(),
    }).ToList();
  }

  public UpsertResult UpsertPrecio(int tratamientoId, int ubicacionId, decimal precio)
  {
    if (ubicacionId <= 0)
    {
      return UpsertResult.Fail(
          "UbicacionId es obligatorio.",
          new Dictionary<string, string> { ["UbicacionId"] = "UbicacionId es obligatorio." });
    }
    if (precio < 0)
    {
      return UpsertResult.Fail(
          "El precio no puede ser negativo.",
          new Dictionary<string, string> { ["Precio"] = "El precio no puede ser negativo." });
    }

    var t = _unitOfWork.TratamientoRepository.GetById(tratamientoId);
    if (t == null) return UpsertResult.Fail("Tratamiento no encontrado.");

    var u = _unitOfWork.UbicacionRepository.GetById(ubicacionId);
    if (u == null) return UpsertResult.Fail("Ubicación no encontrada.");

    var existente = _unitOfWork.TratamientoPrecioRepository
        .All()
        .FirstOrDefault(p => p.TratamientoId == tratamientoId && p.UbicacionId == ubicacionId);

    if (existente == null)
    {
      var nuevo = new TratamientoPrecio
      {
        TratamientoId = tratamientoId,
        UbicacionId = ubicacionId,
        Precio = precio,
      };
      _unitOfWork.TratamientoPrecioRepository.Add(nuevo);
    }
    else
    {
      existente.Precio = precio;
      _unitOfWork.TratamientoPrecioRepository.Upsert(existente);
    }

    _unitOfWork.Save();
    return UpsertResult.Created(0);
  }

  public ServiceResult DeletePrecio(int tratamientoId, int ubicacionId)
  {
    var existente = _unitOfWork.TratamientoPrecioRepository
        .All()
        .FirstOrDefault(p => p.TratamientoId == tratamientoId && p.UbicacionId == ubicacionId);
    if (existente == null) return ServiceResult.Fail("Precio no encontrado.");

    _unitOfWork.TratamientoPrecioRepository.Delete(existente.Id);
    _unitOfWork.Save();
    return ServiceResult.Ok();
  }
}
