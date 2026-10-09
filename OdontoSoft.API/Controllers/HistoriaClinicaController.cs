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
	public class HistoriaClinicaController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public HistoriaClinicaController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/historiaclinica
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var historias = _unitOfWork.HistoriaClinicaRepository.GetAllEnabled().ToList();
				return Ok(historias);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/historiaclinica/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var hc = _unitOfWork.HistoriaClinicaRepository.GetHistoriaClinicaById(id);
				if (hc == null) return ApiErrorResponse.NotFound("Historia clínica no encontrada.");
				return Ok(hc);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/historiaclinica/paciente/5
		[HttpGet("GetByPaciente/{pacienteId}")]
		public IActionResult GetByPaciente(int pacienteId)
		{
			try
			{
				var hc = _unitOfWork.HistoriaClinicaRepository.GetFullByPacienteId(pacienteId);
				if (hc == null) return ApiErrorResponse.NotFound("El paciente no tiene historia clínica.");
				return Ok(hc);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/historiaclinica
		[HttpPost("Create")]
		public IActionResult Create([FromBody] HistoriaClinicaViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (vm.PacienteId == 0)
					fieldErrors["PacienteId"] = "El paciente es obligatorio.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				var existente = _unitOfWork.HistoriaClinicaRepository.GetByPacienteId(vm.PacienteId);
				if (existente != null)
				{
					return ApiErrorResponse.Conflict(
						"El paciente ya tiene una historia clínica creada.",
						new Dictionary<string, string[]>
						{
							["PacienteId"] = new[] { "Este paciente ya tiene historia clínica." }
						});
				}

				var hc = new HistoriaClinica();
				vm.MapViewModelToHistoriaClinica(hc);
				_unitOfWork.HistoriaClinicaRepository.Add(hc);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = hc.Id }, hc);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "HistoriaClinica");
			}
		}

		// PUT api/historiaclinica/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] HistoriaClinicaViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (vm.PacienteId == 0)
					fieldErrors["PacienteId"] = "El paciente es obligatorio.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				var hc = _unitOfWork.HistoriaClinicaRepository.GetById(id);
				if (hc == null) return ApiErrorResponse.NotFound("Historia clínica no encontrada.");

				vm.Id = id;
				vm.MapViewModelToHistoriaClinica(hc);
				_unitOfWork.HistoriaClinicaRepository.Upsert(hc);
				_unitOfWork.Save();
				return Ok(hc);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "HistoriaClinica");
			}
		}

		// DELETE api/historiaclinica/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.HistoriaClinicaRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "HistoriaClinica");
			}
		}

		// ── ODONTOGRAMA ────────────────────────────────────────────────────────────

		// POST api/historiaclinica/pieza
		[HttpPost("GuardarPieza")]
		public IActionResult GuardarPieza([FromBody] OdontogramaPiezaViewModel vm)
		{
			try
			{
				var pieza = _unitOfWork.OdontogramaPiezaRepository
						.GetByHistoriaClinicaIdAndNumeroPieza(vm.HistoriaClinicaId, vm.NumeroPieza);

				if (pieza != null)
				{
					vm.Id = pieza.Id;
					vm.MapViewModelToOdontogramaPieza(pieza);
					_unitOfWork.OdontogramaPiezaRepository.Upsert(pieza);
				}
				else
				{
					pieza = new OdontogramaPieza();
					vm.MapViewModelToOdontogramaPieza(pieza);
					_unitOfWork.OdontogramaPiezaRepository.Add(pieza);
				}

				_unitOfWork.Save();
				return Ok(new { mensaje = "Pieza guardada correctamente" });
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "OdontogramaPieza");
			}
		}

		// ── FICHAS DE TRATAMIENTO ─────────────────────────────────────────────────

		// POST api/historiaclinica/ficha
		[HttpPost("GuardarFicha")]
		public IActionResult GuardarFicha([FromBody] FichaTratamientoViewModel vm)
		{
			try
			{
				FichaTratamiento ficha;
				if (vm.Id != 0)
				{
					ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(vm.Id);
					if (ficha == null) return ApiErrorResponse.NotFound("Ficha de tratamiento no encontrada.");
				}
				else
				{
					ficha = new FichaTratamiento();
					ficha.FechaRealizacion = DateTime.Now;
				}

				vm.MapViewModelToFichaTratamiento(ficha);

				if (vm.Id != 0)
					_unitOfWork.FichaTratamientoRepository.Upsert(ficha);
				else
					_unitOfWork.FichaTratamientoRepository.Add(ficha);

				_unitOfWork.Save();
				return Ok(new
				{
					ficha.Id,
					Tratamiento = ficha.Tratamiento?.Nombre,
					ficha.NumeroPieza,
					ficha.Descripcion,
					FechaRealizacion = ficha.FechaRealizacion.ToString("dd/MM/yyyy")
				});
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "FichaTratamiento");
			}
		}

		// DELETE api/historiaclinica/ficha/5
		[HttpDelete("EliminarFicha/{id}")]
		public IActionResult EliminarFicha(int id)
		{
			try
			{
				_unitOfWork.FichaTratamientoRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "FichaTratamiento");
			}
		}

		// GET api/historiaclinica/tratamientos
		[HttpGet("GetTratamientosParaFicha")]
		public IActionResult GetTratamientosParaFicha()
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
	}
}
