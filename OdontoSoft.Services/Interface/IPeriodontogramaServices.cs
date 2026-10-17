using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio del Periodontograma. Una evaluación periodontograma
/// agrupa 0..32 piezas (de 32 dientes esperados). Las métricas agregadas
/// (BoP%, placa%, presentes/ausentes) se calculan on-the-fly a partir de
/// las piezas — no se persisten.
/// </summary>
public interface IPeriodontogramaServices
{
  /// <summary>
  /// Crea una evaluación periodontograma con sus piezas. Devuelve el VM
  /// con el id asignado, las piezas y las métricas ya calculadas.
  /// </summary>
  UpsertResult<PeriodontogramaDto> Create(PeriodontogramaDto vm);

  /// <summary>
  /// Reemplaza cabecera y todas las piezas de una evaluación existente.
  /// </summary>
  UpsertResult<PeriodontogramaDto> Update(int id, PeriodontogramaDto vm);

  /// <summary>
  /// Elimina la evaluación y todas sus piezas.
  /// </summary>
  ServiceResult Delete(int id);

  /// <summary>
  /// Calcula las métricas agregadas (BoP%, placa%, presentes/ausentes)
  /// a partir de la colección de piezas.
  /// </summary>
  PeriodontogramaMetricasDto CalcularMetricas(ICollection<PeriodontogramaPieza> piezas);
}
