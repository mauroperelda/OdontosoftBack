using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Common;

namespace OdontoSoft.Application.Services;

/// <summary>
/// Lógica de negocio de Tratamientos y sus precios por ubicación.
/// </summary>
public interface ITratamientoServices
{
  /// <summary>
  /// Devuelve los tratamientos con sus precios por ubicación anidados,
  /// consumidos por la pestaña "Servicios" de Ajustes.
  /// </summary>
  IEnumerable<TratamientoConPreciosDto> GetAllConPrecios();

  /// <summary>
  /// Upsert del precio de un tratamiento para una ubicación. Si ya existe,
  /// lo actualiza; si no, lo crea.
  /// </summary>
  UpsertResult UpsertPrecio(int tratamientoId, int ubicacionId, decimal precio);

  /// <summary>
  /// Elimina el precio de un tratamiento para una ubicación.
  /// </summary>
  ServiceResult DeletePrecio(int tratamientoId, int ubicacionId);
}
