using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdontoSoft.API.Helpers;
using OdontoSoft.Contracts.Common;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class PagoController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public PagoController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    // GET api/pago/GetAll?page=1&pageSize=50
    [HttpGet("GetAll")]
    public async Task<ActionResult<PagedResult<PagoHistorialItem>>> GetAll([FromQuery] PaginationParams pagination)
    {
      // Proyectamos a un DTO liviano con el nombre del paciente, así
      // el front no tiene que hacer N+1 llamadas a presupuesto/GetById.
      var query = _unitOfWork.PagoRepository
        .GetAllEnabled()
        .Include(p => p.Presupuesto)
          .ThenInclude(pr => pr.Paciente)
        .OrderByDescending(p => p.Id)
        .Select(p => new PagoHistorialItem
        {
          Id = p.Id,
          PresupuestoId = p.PresupuestoId,
          Fecha = p.Fecha,
          Monto = p.Monto,
          MetodoPago = p.MetodoPago,
          Observaciones = p.Observaciones,
          Estado = p.Presupuesto != null ? p.Presupuesto.Estado : null,
          PacienteNombre = p.Presupuesto != null && p.Presupuesto.Paciente != null
            ? $"{p.Presupuesto.Paciente.Apellido}, {p.Presupuesto.Paciente.Nombre}"
            : null,
        });

      var result = await query.ToPagedResultAsync(pagination);
      return Ok(result);
    }

    // GET api/pago/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Pago> GetById(int id)
    {
      var pago = _unitOfWork.PagoRepository.GetById(id);
      if (pago == null) return ApiErrorResponse.NotFound("Pago no encontrado.");
      return Ok(pago);
    }

    // POST api/pago
    [HttpPost("Create")]
    public ActionResult<Pago> Create([FromBody] PagoDto vm)
    {
      var pago = new Pago();
      vm.MapDtoToPago(pago);
      _unitOfWork.PagoRepository.Add(pago);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = pago.Id }, pago);
    }

    // PUT api/pago/5
    [HttpPut("Update/{id}")]
    public ActionResult<Pago> Update(int id, [FromBody] PagoDto vm)
    {
      var pago = _unitOfWork.PagoRepository.GetById(id);
      if (pago == null) return ApiErrorResponse.NotFound("Pago no encontrado.");
      vm.Id = id;
      vm.MapDtoToPago(pago);
      _unitOfWork.PagoRepository.Upsert(pago);
      _unitOfWork.Save();
      return Ok(pago);
    }

    // DELETE api/pago/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.PagoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }
  }

  // DTO liviano para el historial de pagos. Lo que el front necesita
  // para mostrar la tabla: id, fecha, monto, método, estado del
  // presupuesto y nombre del paciente (evitamos N+1 contra presupuesto).
  public class PagoHistorialItem
  {
    public int Id { get; set; }
    public int PresupuestoId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; }
    public string? Observaciones { get; set; }
    public string? Estado { get; set; }
    public string? PacienteNombre { get; set; }
  }
}
