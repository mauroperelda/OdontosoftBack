using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;

namespace OdontoSoft.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class PresupuestoController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public PresupuestoController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/presupuesto/GetAll?page=1&pageSize=50
		[HttpGet("GetAll")]
		public IActionResult GetAll([FromQuery] PaginationParams pagination)
		{
			try
			{
				var query = _unitOfWork.PresupuestoRepository.GetAllEnabled();
				var total = query.Count();
				var items = query
					.OrderByDescending(p => p.Id)
					.Skip((pagination.Page - 1) * pagination.PageSize)
					.Take(pagination.PageSize)
					.ToList();
				return Ok(new PagedResult<Presupuesto>
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

		// GET api/presupuesto/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
				if (presupuesto == null) return ApiErrorResponse.NotFound("Presupuesto no encontrado.");
				return Ok(presupuesto);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/presupuesto
		[HttpPost("Create")]
		public IActionResult Create([FromBody] PresupuestoViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (vm.PacienteId == 0)
					fieldErrors["PacienteId"] = "El paciente es obligatorio.";

				// ProfesionalId es opcional. Si el front no lo manda o envía 0, lo
				// dejamos en null (la columna en DB es NULL ahora).

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				// Normalizamos 0 → null para no ensuciar la DB con un FK inválido.
				if (vm.ProfesionalId == 0) vm.ProfesionalId = null;

				var presupuesto = new Presupuesto();
				vm.MapViewModelToPresupuesto(presupuesto);
				_unitOfWork.PresupuestoRepository.Add(presupuesto);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = presupuesto.Id }, presupuesto);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Presupuesto");
			}
		}

		// PUT api/presupuesto/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] PresupuestoViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (vm.PacienteId == 0)
					fieldErrors["PacienteId"] = "El paciente es obligatorio.";
				if (vm.ProfesionalId == 0)
					fieldErrors["ProfesionalId"] = "El profesional es obligatorio.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
				if (presupuesto == null) return ApiErrorResponse.NotFound("Presupuesto no encontrado.");

				vm.Id = id;
				vm.MapViewModelToPresupuesto(presupuesto);
				_unitOfWork.PresupuestoRepository.Upsert(presupuesto);
				_unitOfWork.Save();
				return Ok(presupuesto);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Presupuesto");
			}
		}

		// DELETE api/presupuesto/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.PresupuestoRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Presupuesto");
			}
		}

		// PATCH api/presupuesto/5/estado
		[HttpPatch("CambiarEstado/{id}")]
		public IActionResult CambiarEstado(int id, [FromBody] string estado)
		{
			try
			{
				var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(id);
				if (presupuesto == null) return ApiErrorResponse.NotFound("Presupuesto no encontrado.");
				presupuesto.Estado = estado;
				_unitOfWork.PresupuestoRepository.Upsert(presupuesto);
				_unitOfWork.Save();
				return Ok(new { mensaje = "Estado actualizado correctamente" });
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Presupuesto");
			}
		}

		// ── DETALLES ──────────────────────────────────────────────────────────────

		// POST api/presupuesto/detalle
		[HttpPost("GuardarDetallePresupuesto")]
		public IActionResult GuardarDetalle([FromBody] PresupuestoDetalleViewModel vm)
		{
			try
			{
				PresupuestoDetalle detalle;
				if (vm.Id != 0)
				{
					detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(vm.Id);
					if (detalle == null) return ApiErrorResponse.NotFound("Detalle de presupuesto no encontrado.");
				}
				else
				{
					detalle = new PresupuestoDetalle();
				}

				vm.MapViewModelToDetalle(detalle);

				if (vm.Id != 0)
					_unitOfWork.PresupuestoDetalleRepository.Upsert(detalle);
				else
					_unitOfWork.PresupuestoDetalleRepository.Add(detalle);

				_unitOfWork.Save();
				RecalcularTotal(vm.PresupuestoId);

				var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);
				return Ok(new
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
				return ApiErrorResponse.FromException(ex, "PresupuestoDetalle");
			}
		}

		// DELETE api/presupuesto/detalle/5
		[HttpDelete("EliminarDetallePresupuesto/{id}")]
		public IActionResult EliminarDetalle(int id)
		{
			try
			{
				var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
				if (detalle == null) return ApiErrorResponse.NotFound("Detalle de presupuesto no encontrado.");
				int presupuestoId = detalle.PresupuestoId;
				_unitOfWork.PresupuestoDetalleRepository.Delete(id);
				_unitOfWork.Save();
				RecalcularTotal(presupuestoId);
				var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
				return Ok(new { total = presupuesto.Total.ToString("N2") });
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "PresupuestoDetalle");
			}
		}

		// ── PAGOS ─────────────────────────────────────────────────────────────────

		// POST api/presupuesto/pago
		[HttpPost("GuardarPagoPresupuesto")]
		public IActionResult GuardarPago([FromBody] PagoViewModel vm)
		{
			try
			{
				var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(vm.PresupuestoId);
				if (presupuesto == null) return ApiErrorResponse.NotFound("Presupuesto no encontrado.");

				decimal totalPagado = presupuesto.Pagos?.Sum(p => p.Monto) ?? 0;
				decimal saldoPendiente = presupuesto.Total - totalPagado;

				if (vm.Monto > saldoPendiente)
				{
					return ApiErrorResponse.BadRequest(
						$"El monto supera el saldo pendiente de ${saldoPendiente:N2}.",
						new Dictionary<string, string[]>
						{
							["Monto"] = new[] { $"El máximo permitido es ${saldoPendiente:N2}." }
						});
				}

				Pago pago;
				if (vm.Id != 0)
				{
					pago = _unitOfWork.PagoRepository.GetById(vm.Id);
					if (pago == null) return ApiErrorResponse.NotFound("Pago no encontrado.");
				}
				else
				{
					pago = new Pago();
				}

				vm.MapViewModelToPago(pago);

				if (vm.Id != 0)
					_unitOfWork.PagoRepository.Upsert(pago);
				else
					_unitOfWork.PagoRepository.Add(pago);

				_unitOfWork.Save();

				totalPagado += vm.Monto;
				if (totalPagado >= presupuesto.Total)
				{
					presupuesto.Estado = "Pagado";
					_unitOfWork.PresupuestoRepository.Upsert(presupuesto);
					_unitOfWork.Save();
				}

				decimal nuevoSaldo = presupuesto.Total - totalPagado;
				return Ok(new
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
				return ApiErrorResponse.FromException(ex, "Pago");
			}
		}

		// DELETE api/presupuesto/pago/5
		[HttpDelete("EliminarPagoPresupuesto/{id}")]
		public IActionResult EliminarPago(int id)
		{
			try
			{
				var pago = _unitOfWork.PagoRepository.GetById(id);
				if (pago == null) return ApiErrorResponse.NotFound("Pago no encontrado.");
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

				return Ok(new
				{
					TotalPagado = totalPagado.ToString("N2"),
					SaldoPendiente = saldoPendiente.ToString("N2"),
					EstadoPresupuesto = presupuesto.Estado
				});
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Pago");
			}
		}

		// GET api/presupuesto/tratamientos
		[HttpGet("GetTratamientosParaDetalle")]
		public IActionResult GetTratamientosParaDetalle()
		{
			try
			{
				var tratamientos = _unitOfWork.TratamientoRepository
						.GetAllEnabled()
						.Select(t => new { t.Id, t.Nombre })
						.ToList();
				return Ok(tratamientos);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// ── HELPERS ───────────────────────────────────────────────────────────────

		private void RecalcularTotal(int presupuestoId)
		{
			var presupuesto = _unitOfWork.PresupuestoRepository.GetPresupuestoById(presupuestoId);
			presupuesto.Total = presupuesto.Detalles?.Sum(d => d.Subtotal) ?? 0;
			_unitOfWork.PresupuestoRepository.Upsert(presupuesto);
			_unitOfWork.Save();
		}
	}
}
