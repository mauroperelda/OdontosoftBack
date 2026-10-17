using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services.Common;
using Odontosoft.Services;

namespace OdontoSoft.Services;

/// <summary>
/// Lógica de negocio de Presupuestos. Extraída de PresupuestoController.
/// Mantiene intacta la semántica existente: normalizaciones, recálculo de
/// totales al modificar detalles, validación de saldo al pagar, y auto-flip
/// Pagado ↔ Pendiente.
/// </summary>
public partial class PresupuestoServices
{
  public UpsertResult Create(PresupuestoDto vm)
  {
    if (vm.ProfesionalId == 0) vm.ProfesionalId = null;

    var presupuesto = new Presupuesto();
    vm.MapDtoToPresupuesto(presupuesto);
    _unitOfWork.PresupuestoRepository.Add(presupuesto);
    _unitOfWork.Save();
    return UpsertResult.Created(presupuesto.Id);
  }

  public UpsertResult Update(int id, PresupuestoDto vm)
  {
    if (vm.ProfesionalId == null || vm.ProfesionalId == 0)
    {
      return UpsertResult.Fail(
          "El profesional es obligatorio.",
          new Dictionary<string, string> { ["ProfesionalId"] = "El profesional es obligatorio." });
    }

    var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
    if (presupuesto == null)
    {
      return UpsertResult.Fail("Presupuesto no encontrado.");
    }

    vm.Id = id;
    vm.MapDtoToPresupuesto(presupuesto);
    _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
    _unitOfWork.Save();
    return UpsertResult.Created(presupuesto.Id);
  }

  public UpsertResult GuardarDetalle(PresupuestoDetalleDto vm)
  {
    PresupuestoDetalle detalle;
    if (vm.Id != 0)
    {
      detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(vm.Id);
      if (detalle == null) return UpsertResult.Fail("Detalle de presupuesto no encontrado.");
    }
    else
    {
      detalle = new PresupuestoDetalle();
    }

    vm.MapDtoToDetalle(detalle);

    if (vm.Id != 0)
      _unitOfWork.PresupuestoDetalleRepository.Upsert(detalle);
    else
      _unitOfWork.PresupuestoDetalleRepository.Add(detalle);

    _unitOfWork.Save();
    RecalcularTotal(vm.PresupuestoId);
    return UpsertResult.Created(detalle.Id);
  }

  public ServiceResult EliminarDetalle(int detalleId)
  {
    var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(detalleId);
    if (detalle == null) return ServiceResult.Fail("Detalle de presupuesto no encontrado.");

    int presupuestoId = detalle.PresupuestoId;
    _unitOfWork.PresupuestoDetalleRepository.Delete(detalleId);
    _unitOfWork.Save();
    RecalcularTotal(presupuestoId);
    return ServiceResult.Ok();
  }

  public UpsertResult GuardarPago(PagoDto vm)
  {
    var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);
    if (presupuesto == null) return UpsertResult.Fail("Presupuesto no encontrado.");

    decimal totalPagado = presupuesto.Pagos?.Sum(p => p.Monto) ?? 0;
    decimal saldoPendiente = presupuesto.Total - totalPagado;

    if (vm.Monto > saldoPendiente)
    {
      return UpsertResult.Fail(
          $"El monto supera el saldo pendiente de ${saldoPendiente:N2}.",
          new Dictionary<string, string>
          {
            ["Monto"] = $"El máximo permitido es ${saldoPendiente:N2}."
          });
    }

    Pago pago;
    if (vm.Id != 0)
    {
      pago = _unitOfWork.PagoRepository.GetById(vm.Id);
      if (pago == null) return UpsertResult.Fail("Pago no encontrado.");
    }
    else
    {
      pago = new Pago();
    }

    vm.MapDtoToPago(pago);

    if (vm.Id != 0)
      _unitOfWork.PagoRepository.Upsert(pago);
    else
      _unitOfWork.PagoRepository.Add(pago);

    _unitOfWork.Save();

    totalPagado += vm.Monto;
    if (totalPagado >= presupuesto.Total && presupuesto.Total > 0)
    {
      presupuesto.Estado = "Pagado";
      _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
      _unitOfWork.Save();
    }

    return UpsertResult.Created(pago.Id);
  }

  public ServiceResult EliminarPago(int pagoId)
  {
    var pago = _unitOfWork.PagoRepository.GetById(pagoId);
    if (pago == null) return ServiceResult.Fail("Pago no encontrado.");

    int presupuestoId = pago.PresupuestoId;
    _unitOfWork.PagoRepository.Delete(pagoId);
    _unitOfWork.Save();

    var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
    decimal totalPagado = presupuesto?.Pagos?.Sum(p => p.Monto) ?? 0;
    decimal saldoPendiente = (presupuesto?.Total ?? 0) - totalPagado;

    if (presupuesto != null && presupuesto.Estado == "Pagado" && saldoPendiente > 0)
    {
      presupuesto.Estado = "Pendiente";
      _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
      _unitOfWork.Save();
    }

    return ServiceResult.Ok();
  }

  public ServiceResult CambiarEstado(int id, string estado)
  {
    var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
    if (presupuesto == null) return ServiceResult.Fail("Presupuesto no encontrado.");

    presupuesto.Estado = estado;
    _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
    _unitOfWork.Save();
    return ServiceResult.Ok();
  }

  public RecalcularTotalResult RecalcularTotal(int presupuestoId)
  {
    var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
    if (presupuesto == null) return RecalcularTotalResult.Ok(0, null);

    presupuesto.Total = presupuesto.Detalles?.Sum(d => d.Subtotal) ?? 0;
    _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
    _unitOfWork.Save();
    return RecalcularTotalResult.Ok(presupuesto.Total, presupuesto.Estado);
  }
}
