using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio del Odontograma (piezas dentales). Una pieza es única
/// por la combinación (HistoriaClinicaId, NumeroPieza), así que el upsert
/// es por esa llave compuesta.
/// </summary>
public interface IOdontogramaPiezaServices
{
  /// <summary>
  /// Crea la pieza o, si ya existe una para esa (HC, NumeroPieza), la actualiza.
  /// Devuelve Ok con un mensaje para que el controller lo devuelva como JSON.
  /// </summary>
  ServiceResult UpsertByPieza(OdontogramaPiezaDto vm);
}
