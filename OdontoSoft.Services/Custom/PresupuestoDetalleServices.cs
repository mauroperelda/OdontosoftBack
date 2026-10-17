using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Services.Common;
using Odontosoft.Services;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio de Detalles de Presupuesto.
/// </summary>
public partial class PresupuestoDetalleServices
{
  public decimal CalcularSubtotal(decimal cantidad, decimal precioUnitario) =>
      cantidad * precioUnitario;

  public ServiceResult ValidarCantidad(decimal cantidad)
  {
    if (cantidad <= 0)
      return ServiceResult.Fail(
          "La cantidad debe ser mayor a 0.",
          new Dictionary<string, string> { ["Cantidad"] = "La cantidad debe ser mayor a 0." });
    return ServiceResult.Ok();
  }
}
