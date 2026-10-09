using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;

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
		public IActionResult GetAll([FromQuery] PaginationParams pagination)
		{
			try
			{
				// Proyectamos a un DTO liviano con el nombre del paciente, así
				// el front no tiene que hacer N+1 llamadas a presupuesto/GetById.
				var query = _unitOfWork.PagoRepository
					.GetAllEnabled()
					.Include(p => p.Presupuesto)
						.ThenInclude(pr => pr.Paciente);

				var total = query.Count();
				var items = query
					.OrderByDescending(p => p.Id)
					.Skip((pagination.Page - 1) * pagination.PageSize)
					.Take(pagination.PageSize)
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
					})
					.ToList();

				return Ok(new PagedResult<PagoHistorialItem>
				{
					Items = items,
					Total = total,
					Page = pagination.Page,
					PageSize = pagination.PageSize,
				});
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/pago/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var pago = _unitOfWork.PagoRepository.GetById(id);
				if (pago == null) return ApiErrorResponse.NotFound("Pago no encontrado.");
				return Ok(pago);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/pago
		[HttpPost("Create")]
		public IActionResult Create([FromBody] PagoViewModel vm)
		{
			try
			{
				var pago = new Pago();
				vm.MapViewModelToPago(pago);
				_unitOfWork.PagoRepository.Add(pago);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = pago.Id }, pago);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Pago");
			}
		}

		// PUT api/pago/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] PagoViewModel vm)
		{
			try
			{
				var pago = _unitOfWork.PagoRepository.GetById(id);
				if (pago == null) return ApiErrorResponse.NotFound("Pago no encontrado.");
				vm.Id = id;
				vm.MapViewModelToPago(pago);
				_unitOfWork.PagoRepository.Upsert(pago);
				_unitOfWork.Save();
				return Ok(pago);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Pago");
			}
		}

		// DELETE api/pago/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.PagoRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Pago");
			}
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
