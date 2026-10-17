using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Contracts.Common;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Services;
using OdontoSoft.Application.Common;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class PresupuestoController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPresupuestoServices _presupuestoServices;

    public PresupuestoController(IUnitOfWork unitOfWork, IPresupuestoServices presupuestoServices)
    {
      _unitOfWork = unitOfWork;
      _presupuestoServices = presupuestoServices;
    }

    // GET api/presupuesto/GetAll?page=1&pageSize=50
    [HttpGet("GetAll")]
    public async Task<ActionResult<PagedResult<Presupuesto>>> GetAll([FromQuery] PaginationParams pagination)
    {
      var result = await _unitOfWork.PresupuestoRepository
        .GetAllEnabled()
        .OrderByDescending(p => p.Id)
        .ToPagedResultAsync(pagination);
      return Ok(result);
    }

    // GET api/presupuesto/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Presupuesto> GetById(int id)
    {
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
      if (presupuesto == null) return ApiErrorResponse.NotFound("Presupuesto no encontrado.");
      return Ok(presupuesto);
    }

    // POST api/presupuesto
    // Validación de shape delegada a PresupuestoDtoValidator.
    [HttpPost("Create")]
    public ActionResult<Presupuesto> Create([FromBody] PresupuestoDto vm)
    {
      var result = _presupuestoServices.Create(vm);
      if (!result.IsSuccess) return BadRequest(result);
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(result.Id!.Value);
      return CreatedAtAction(nameof(GetById), new { id = result.Id }, presupuesto);
    }

    // PUT api/presupuesto/5
    [HttpPut("Update/{id}")]
    public ActionResult<Presupuesto> Update(int id, [FromBody] PresupuestoDto vm)
    {
      var result = _presupuestoServices.Update(id, vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Presupuesto no encontrado.")
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
      return Ok(presupuesto);
    }

    // DELETE api/presupuesto/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.PresupuestoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // PATCH api/presupuesto/5/estado
    [HttpPatch("CambiarEstado/{id}")]
    public IActionResult CambiarEstado(int id, [FromBody] string estado)
    {
      var result = _presupuestoServices.CambiarEstado(id, estado);
      if (!result.IsSuccess) return ApiErrorResponse.NotFound(result.Error!);
      return Ok(new { mensaje = "Estado actualizado correctamente" });
    }

    // POST api/presupuesto/detalle
    [HttpPost("GuardarDetallePresupuesto")]
    public IActionResult GuardarDetalle([FromBody] PresupuestoDetalleDto vm)
    {
      var result = _presupuestoServices.GuardarDetalle(vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Detalle de presupuesto no encontrado.")
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }
      var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(result.Id!.Value);
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);
      return Ok(new
      {
        detalle!.Id,
        Tratamiento = detalle.Tratamiento?.Nombre,
        detalle.NumeroPieza,
        detalle.Cantidad,
        PrecioUnitario = detalle.PrecioUnitario.ToString("N2"),
        Subtotal = detalle.Subtotal.ToString("N2"),
        TotalPresupuesto = presupuesto!.Total.ToString("N2")
      });
    }

    // DELETE api/presupuesto/detalle/5
    [HttpDelete("EliminarDetallePresupuesto/{id}")]
    public IActionResult EliminarDetalle(int id)
    {
      var result = _presupuestoServices.EliminarDetalle(id);
      if (!result.IsSuccess) return ApiErrorResponse.NotFound(result.Error!);
      var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
      int presupuestoId = detalle?.PresupuestoId ?? 0;
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
      return Ok(new { total = presupuesto!.Total.ToString("N2") });
    }

    // POST api/presupuesto/pago
    [HttpPost("GuardarPagoPresupuesto")]
    public IActionResult GuardarPago([FromBody] PagoDto vm)
    {
      var result = _presupuestoServices.GuardarPago(vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Presupuesto no encontrado." || result.Error == "Pago no encontrado.")
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }
      var pago = _unitOfWork.PagoRepository.GetById(result.Id!.Value);
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);
      decimal totalPagado = presupuesto!.Pagos?.Sum(p => p.Monto) ?? 0;
      decimal nuevoSaldo = presupuesto.Total - totalPagado;
      return Ok(new
      {
        pago!.Id,
        Fecha = pago.Fecha.ToString("dd/MM/yyyy"),
        Monto = pago.Monto.ToString("N2"),
        pago.MetodoPago,
        pago.Observaciones,
        TotalPagado = totalPagado.ToString("N2"),
        SaldoPendiente = nuevoSaldo.ToString("N2"),
        EstadoPresupuesto = presupuesto.Estado
      });
    }

    // DELETE api/presupuesto/pago/5
    [HttpDelete("EliminarPagoPresupuesto/{id}")]
    public IActionResult EliminarPago(int id)
    {
      var result = _presupuestoServices.EliminarPago(id);
      if (!result.IsSuccess) return ApiErrorResponse.NotFound(result.Error!);
      var pago = _unitOfWork.PagoRepository.GetById(id);
      int presupuestoId = pago?.PresupuestoId ?? 0;
      var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
      decimal totalPagado = presupuesto!.Pagos?.Sum(p => p.Monto) ?? 0;
      decimal saldoPendiente = presupuesto.Total - totalPagado;
      return Ok(new
      {
        TotalPagado = totalPagado.ToString("N2"),
        SaldoPendiente = saldoPendiente.ToString("N2"),
        EstadoPresupuesto = presupuesto.Estado
      });
    }

    // GET api/presupuesto/tratamientos
    [HttpGet("GetTratamientosParaDetalle")]
    public IActionResult GetTratamientosParaDetalle()
    {
      var tratamientos = _unitOfWork.TratamientoRepository
          .GetAllEnabled()
          .Select(t => new { t.Id, t.Nombre })
          .ToList();
      return Ok(tratamientos);
    }

    // Traduce un ServiceResult fallido a IActionResult. Mantiene
    // compatibilidad con la shape { error, errors } que el front React espera.
    private ActionResult BadRequest(ServiceResult result)
    {
      var fieldErrors = result.FieldErrors?
        .ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
      return ApiErrorResponse.BadRequest(result.Error ?? "Operación inválida.", fieldErrors);
    }
  }
}

