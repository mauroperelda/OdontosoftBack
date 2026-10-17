using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio de Historia Clínica. Extraída de HistoriaClinicaController.
/// La HC es única por paciente; las piezas del odontograma y las fichas de
/// tratamiento son hijos que viven bajo su HC.
/// </summary>
public interface IHistoriaClinicaServices
{
  /// <summary>
  /// Crea una HC. Falla si el paciente ya tiene una.
  /// </summary>
  UpsertResult Create(HistoriaClinicaDto vm);

  /// <summary>
  /// Actualiza una HC existente.
  /// </summary>
  UpsertResult Update(int id, HistoriaClinicaDto vm);

  /// <summary>
  /// Upsert de una pieza del odontograma. Si ya existe una pieza para esa
  /// combinación (HC, NumeroPieza), la actualiza; si no, la crea.
  /// </summary>
  ServiceResult GuardarPieza(OdontogramaPiezaDto vm);

  /// <summary>
  /// Crea o actualiza una ficha de tratamiento asociada a la HC.
  /// </summary>
  UpsertResult GuardarFicha(FichaTratamientoDto vm);

  /// <summary>
  /// Elimina una ficha de tratamiento por id.
  /// </summary>
  ServiceResult EliminarFicha(int fichaId);

  /// <summary>
  /// Devuelve la lista de tratamientos habilitados (id + nombre) para
  /// alimentar el dropdown del formulario de ficha.
  /// </summary>
  IEnumerable<(int Id, string Nombre)> GetTratamientosParaFicha();
}
