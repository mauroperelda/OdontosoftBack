using OdontoSoft.Services.Common;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio de Detalles de Presupuesto.
/// </summary>
public interface IPresupuestoDetalleServices
{
  /// <summary>
  /// Calcula el subtotal = cantidad × precioUnitario. Regla de negocio
  /// que también está reflejada en ApplicationProfile (AutoMapper), pero
  /// se expone acá para uso directo desde el service.
  /// </summary>
  decimal CalcularSubtotal(decimal cantidad, decimal precioUnitario);

  /// <summary>
  /// Valida que la cantidad sea estrictamente mayor a 0.
  /// </summary>
  ServiceResult ValidarCantidad(decimal cantidad);
}
