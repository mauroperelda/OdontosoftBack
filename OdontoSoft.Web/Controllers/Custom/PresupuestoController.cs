using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Web.Controllers
{
  public partial class PresupuestoController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(Presupuesto entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      SelectList pacienteList = new SelectList(_unitOfWork.PacienteRepository.GetAllEnabled(entity.PacienteId).ToList(), "Id", "GetString", entity.PacienteId);
      viewBagData.Add("paciente", pacienteList);
      //SelectList profesionalList = new SelectList(_unitOfWork.ProfesionalRepository.GetAllEnabled(entity.ProfesionalId).ToList(), "Id", "GetString", entity.ProfesionalId);
      //viewBagData.Add("profesional", profesionalList);
      return viewBagData;
    }

    private void PrepareEntityFromFormData(Presupuesto entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
    }

    private void PrepareEntityBeforeLoadForm(Presupuesto entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
    }

    private void ActionsBeforeSave(Presupuesto entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Presupuesto entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }

    [HttpGet]
    public IActionResult Edit(int id = 0, int pacienteId = 0)
    {
      PresupuestoDto vm;

      if (id != 0)
      {
        var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
        vm = new PresupuestoDto(presupuesto);
      }
      else
      {
        vm = new PresupuestoDto();
        vm.Fecha = DateTime.Today;
        vm.Estado = "Pendiente";
      }

      CargarViewBags(vm);
      return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(PresupuestoDto vm)
    {
      if (vm.PacienteId == 0)
        ModelState.AddModelError(string.Empty, "El campo paciente no puede estar vacío.");
      if (vm.ProfesionalId == 0)
        ModelState.AddModelError(string.Empty, "El campo profesional no puede estar vacío.");

      CargarViewBags(vm);

      if (ModelState.IsValid)
      {
        try
        {
          Presupuesto presupuesto;
          if (vm.Id != 0)
            presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.Id);
          else
            presupuesto = new Presupuesto();

          vm.MapDtoToPresupuesto(presupuesto);

          if (vm.Id != 0)
            _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
          else
            _unitOfWork.PresupuestoRepository.Add(presupuesto);

          _unitOfWork.Save();

          return RedirectToAction("Edit", new { id = presupuesto.Id });
        }
        catch (Exception ex)
        {
          ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
        }
      }

      return View(vm);
    }

    [HttpGet]
    public IActionResult EditByPaciente(int id)
    {
      return RedirectToAction("Edit", new { pacienteId = id });
    }

    // ── DETALLES ─────────────────────────────────────────────

    [HttpPost]
    public JsonResult GuardarDetalle([FromBody] PresupuestoDetalleDto vm)
    {
      try
      {
        PresupuestoDetalle detalle;
        if (vm.Id != 0)
        {
          detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(vm.Id);
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

        // Recalcular total del presupuesto
        RecalcularTotal(vm.PresupuestoId);

        var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);

        return JsonOk("detalle", new
        {
          detalle.Id,
          Tratamiento = detalle.Tratamiento?.Nombre,
          detalle.NumeroPieza,
          detalle.Cantidad,
          PrecioUnitario = detalle.PrecioUnitario.ToString("N2"),
          Subtotal = detalle.Subtotal.ToString("N2"),
          TotalPresupuesto = presupuesto.Total.ToString("N2")
        });
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    [HttpPost]
    public JsonResult EliminarDetalle(int id)
    {
      try
      {
        var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
        int presupuestoId = detalle.PresupuestoId;
        _unitOfWork.PresupuestoDetalleRepository.Delete(id);
        _unitOfWork.Save();
        RecalcularTotal(presupuestoId);
        var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
        return JsonOk("total", presupuesto.Total.ToString("N2"));
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    // ── PAGOS ─────────────────────────────────────────────────

    [HttpPost]
    public JsonResult GuardarPago([FromBody] PagoDto vm)
    {
      try
      {
        var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);
        decimal totalPagado = presupuesto.Pagos?.Sum(p => p.Monto) ?? 0;
        decimal saldoPendiente = presupuesto.Total - totalPagado;

        if (vm.Monto > saldoPendiente)
          return JsonError("El monto supera el saldo pendiente de $" + saldoPendiente.ToString("N2"));

        Pago pago;
        if (vm.Id != 0)
          pago = _unitOfWork.PagoRepository.GetById(vm.Id);
        else
          pago = new Pago();

        vm.MapDtoToPago(pago);

        if (vm.Id != 0)
          _unitOfWork.PagoRepository.Upsert(pago);
        else
          _unitOfWork.PagoRepository.Add(pago);

        _unitOfWork.Save();

        // Actualizar estado del presupuesto si está completamente pagado
        totalPagado += vm.Monto;
        if (totalPagado >= presupuesto.Total)
        {
          presupuesto.Estado = "Pagado";
          _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
          _unitOfWork.Save();
        }

        decimal nuevoSaldo = presupuesto.Total - totalPagado;

        return JsonOk("pago", new
        {
          pago.Id,
          Fecha = pago.Fecha.ToString("dd/MM/yyyy"),
          Monto = pago.Monto.ToString("N2"),
          pago.MetodoPago,
          pago.Observaciones,
          TotalPagado = totalPagado.ToString("N2"),
          SaldoPendiente = nuevoSaldo.ToString("N2"),
          EstadoPresupuesto = presupuesto.Estado
        });
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    [HttpPost]
    public JsonResult EliminarPago(int id)
    {
      try
      {
        var pago = _unitOfWork.PagoRepository.GetById(id);
        int presupuestoId = pago.PresupuestoId;
        _unitOfWork.PagoRepository.Delete(id);
        _unitOfWork.Save();

        var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
        decimal totalPagado = presupuesto.Pagos?.Sum(p => p.Monto) ?? 0;
        decimal saldoPendiente = presupuesto.Total - totalPagado;

        if (presupuesto.Estado == "Pagado" && saldoPendiente > 0)
        {
          presupuesto.Estado = "Pendiente";
          _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
          _unitOfWork.Save();
        }

        return JsonOk("saldo", new
        {
          TotalPagado = totalPagado.ToString("N2"),
          SaldoPendiente = saldoPendiente.ToString("N2"),
          EstadoPresupuesto = presupuesto.Estado
        });
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    [HttpPost]
    public JsonResult CambiarEstado(int id, string estado)
    {
      try
      {
        var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
        presupuesto.Estado = estado;
        _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
        _unitOfWork.Save();
        return JsonOk();
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    [HttpGet]
    public JsonResult GetTratamientosParaDetalle()
    {
      try
      {
        var tratamientos = _unitOfWork.TratamientoRepository
            .GetAllEnabled()
            .Select(t => new { t.Id, t.Nombre })
            .ToList();
        return JsonOk("tratamientos", tratamientos);
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    // ── HELPERS ───────────────────────────────────────────────

    private void RecalcularTotal(int presupuestoId)
    {
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
      presupuesto.Total = presupuesto.Detalles?.Sum(d => d.Subtotal) ?? 0;
      _unitOfWork.PresupuestoRepository.Upsert(presupuesto);
      _unitOfWork.Save();
    }

    private void CargarViewBags(PresupuestoDto vm)
    {
      ViewBag.Profesionales = new SelectList(_unitOfWork.ProfesionalRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.ProfesionalId);
    }
  }
}

