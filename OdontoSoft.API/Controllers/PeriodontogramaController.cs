using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;

namespace OdontoSoft.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class PeriodontogramaController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public PeriodontogramaController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/periodontograma
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var items = _unitOfWork.PeriodontogramaRepository
					.GetAllEnabled()
					.OrderByDescending(x => x.FechaRegistro)
					.ToList();
				return Ok(items.Select(p => new PeriodontogramaViewModel(p)).ToList());
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/periodontograma/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var p = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
				if (p == null) return ApiErrorResponse.NotFound("Periodontograma no encontrado.");
				var vm = new PeriodontogramaViewModel(p);
				vm.Metricas = CalcularMetricas(p.Piezas);
				return Ok(vm);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/periodontograma/historiaclinica/5
		// Devuelve la última evaluación con piezas + métricas calculadas.
		[HttpGet("GetByHistoriaClinica/{historiaClinicaId}")]
		public IActionResult GetByHistoriaClinica(int historiaClinicaId)
		{
			try
			{
				var p = _unitOfWork.PeriodontogramaRepository
					.GetLastByHistoriaClinicaId(historiaClinicaId);
				if (p == null) return Ok(null);
				var vm = new PeriodontogramaViewModel(p);
				vm.Metricas = CalcularMetricas(p.Piezas);
				return Ok(vm);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/periodontograma/metricas/5
		[HttpGet("GetMetricas/{id}")]
		public IActionResult GetMetricas(int id)
		{
			try
			{
				var p = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
				if (p == null) return ApiErrorResponse.NotFound("Periodontograma no encontrado.");
				return Ok(CalcularMetricas(p.Piezas));
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/periodontograma
		[HttpPost("Create")]
		public IActionResult Create([FromBody] PeriodontogramaViewModel vm)
		{
			try
			{
				if (vm.HistoriaClinicaId <= 0)
					return ApiErrorResponse.BadRequest("Falta HistoriaClinicaId.");

				var entity = new Periodontograma
				{
					HistoriaClinicaId = vm.HistoriaClinicaId,
					FechaRegistro = vm.FechaRegistro == default ? DateTime.UtcNow : vm.FechaRegistro,
					Observaciones = vm.Observaciones,
				};
				_unitOfWork.PeriodontogramaRepository.Add(entity);
				_unitOfWork.Save();

				if (vm.Piezas != null)
				{
					foreach (var piezaVm in vm.Piezas)
					{
						var pieza = new PeriodontogramaPieza
						{
							PeriodontogramaId = entity.Id,
							NumeroPieza = piezaVm.NumeroPieza,
							Estado = piezaVm.Estado,
							Sangrado = piezaVm.Sangrado,
							Supuracion = piezaVm.Supuracion,
							Placa = piezaVm.Placa,
							BolsaPeriodontal = piezaVm.BolsaPeriodontal,
							Recesion = piezaVm.Recesion,
							Observacion = piezaVm.Observacion,
						};
						_unitOfWork.PeriodontogramaRepository.AddPieza(pieza);
					}
					_unitOfWork.Save();
				}

				var loaded = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(entity.Id);
				var result = new PeriodontogramaViewModel(loaded);
				result.Metricas = CalcularMetricas(loaded.Piezas);
				return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Periodontograma");
			}
		}

		// PUT api/periodontograma/5
		// Reemplaza cabecera y todas sus piezas.
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] PeriodontogramaViewModel vm)
		{
			try
			{
				var entity = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
				if (entity == null) return ApiErrorResponse.NotFound("Periodontograma no encontrado.");

				entity.Observaciones = vm.Observaciones;
				if (vm.FechaRegistro != default) entity.FechaRegistro = vm.FechaRegistro;
				_unitOfWork.PeriodontogramaRepository.Upsert(entity);

				// Reemplazar piezas: borrar existentes, insertar nuevas.
				_unitOfWork.PeriodontogramaRepository.DeletePiezas(entity.Id);
				if (vm.Piezas != null)
				{
					foreach (var piezaVm in vm.Piezas)
					{
						var pieza = new PeriodontogramaPieza
						{
							PeriodontogramaId = entity.Id,
							NumeroPieza = piezaVm.NumeroPieza,
							Estado = piezaVm.Estado,
							Sangrado = piezaVm.Sangrado,
							Supuracion = piezaVm.Supuracion,
							Placa = piezaVm.Placa,
							BolsaPeriodontal = piezaVm.BolsaPeriodontal,
							Recesion = piezaVm.Recesion,
							Observacion = piezaVm.Observacion,
						};
						_unitOfWork.PeriodontogramaRepository.AddPieza(pieza);
					}
				}
				_unitOfWork.Save();

				var loaded = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
				var result = new PeriodontogramaViewModel(loaded);
				result.Metricas = CalcularMetricas(loaded.Piezas);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Periodontograma");
			}
		}

		// DELETE api/periodontograma/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				var entity = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
				if (entity == null) return ApiErrorResponse.NotFound("Periodontograma no encontrado.");
				_unitOfWork.PeriodontogramaRepository.DeletePiezas(entity.Id);
				_unitOfWork.PeriodontogramaRepository.Delete(entity.Id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Periodontograma");
			}
		}

		// ── Helpers ───────────────────────────────────────────────────────

		private static PeriodontogramaMetricasViewModel CalcularMetricas(ICollection<PeriodontogramaPieza> piezas)
		{
			const int totalEsperado = 32;
			if (piezas == null || piezas.Count == 0)
			{
				return new PeriodontogramaMetricasViewModel
				{
					BopPorcentaje = 0,
					PlacaPorcentaje = 0,
					DientesPresentes = 0,
					DientesAusentes = totalEsperado,
				};
			}

			int total = piezas.Count;
			int conSangrado = piezas.Count(p => p.Sangrado);
			int conPlaca = piezas.Count(p => p.Placa);

			return new PeriodontogramaMetricasViewModel
			{
				BopPorcentaje = total == 0 ? 0 : Math.Round((decimal)conSangrado * 100 / total, 2),
				PlacaPorcentaje = total == 0 ? 0 : Math.Round((decimal)conPlaca * 100 / total, 2),
				DientesPresentes = total,
				DientesAusentes = Math.Max(0, totalEsperado - total),
			};
		}
	}
}
