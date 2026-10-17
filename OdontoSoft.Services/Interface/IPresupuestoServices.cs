using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio de Presupuestos. Extraída de PresupuestoController.
/// </summary>
public interface IPresupuestoServices
{
  /// <summary>
  /// Crea un presupuesto. Normaliza ProfesionalId = 0 a null.
  /// </summary>
  UpsertResult Create(PresupuestoDto vm);

  /// <summary>
  /// Actualiza un presupuesto. Valida que ProfesionalId no sea null/0.
  /// </summary>
  UpsertResult Update(int id, PresupuestoDto vm);

  /// <summary>
  /// Crea o actualiza un detalle de presupuesto. Tras guardar, recalcula
  /// el total del presupuesto padre.
  /// </summary>
  UpsertResult GuardarDetalle(PresupuestoDetalleDto vm);

  /// <summary>
  /// Elimina un detalle y recalcula el total del presupuesto.
  /// </summary>
  ServiceResult EliminarDetalle(int detalleId);

  /// <summary>
  /// Crea o actualiza un pago. Valida que el monto no supere el saldo
  /// pendiente. Tras guardar, si el total pagado alcanza el total del
  /// presupuesto, marca el estado como "Pagado".
  /// </summary>
  UpsertResult GuardarPago(PagoDto vm);

  /// <summary>
  /// Elimina un pago. Si el presupuesto estaba en "Pagado" y ahora tiene
  /// saldo pendiente, lo vuelve a "Pendiente".
  /// </summary>
  ServiceResult EliminarPago(int pagoId);

  /// <summary>
  /// Cambia el estado del presupuesto (PATCH endpoint).
  /// </summary>
  ServiceResult CambiarEstado(int id, string estado);

  /// <summary>
  /// Recalcula el total de un presupuesto a partir de sus detalles
  /// y, si corresponde, flipea el estado a "Pagado".
  /// </summary>
  RecalcularTotalResult RecalcularTotal(int presupuestoId);
}
