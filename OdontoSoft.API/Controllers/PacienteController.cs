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
	public class PacienteController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public PacienteController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/paciente/GetAll?page=1&pageSize=50
		[HttpGet("GetAll")]
		public IActionResult GetAll([FromQuery] PaginationParams pagination)
		{
			try
			{
				var query = _unitOfWork.PacienteRepository.GetAllEnabled();
				var total = query.Count();
				var items = query
					.Skip((pagination.Page - 1) * pagination.PageSize)
					.Take(pagination.PageSize)
					.ToList();
				return Ok(new PagedResult<Paciente>
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

		// GET api/paciente/GetById/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var paciente = _unitOfWork.PacienteRepository.GetById(id);
				if (paciente == null) return ApiErrorResponse.NotFound("Paciente no encontrado.");
				return Ok(paciente);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/paciente/Create
		[HttpPost("Create")]
		public IActionResult Create([FromBody] PacienteViewModel vm)
		{
			try
			{
				// ── Validación de negocio ────────────────────────────────────
				// Los errores van al ModelState para que
				// InvalidModelStateResponseFactory los serialice con el
				// formato { error, errors: { Campo: ["..."] } }.
				var fieldErrors = new Dictionary<string, string>();
				if (string.IsNullOrWhiteSpace(vm.Nombre))
					fieldErrors["Nombre"] = "El nombre es obligatorio.";
				if (string.IsNullOrWhiteSpace(vm.Apellido))
					fieldErrors["Apellido"] = "El apellido es obligatorio.";
				if (string.IsNullOrWhiteSpace(vm.Dni))
					fieldErrors["Dni"] = "El DNI es obligatorio.";
				if (vm.FechaNacimiento is null)
					fieldErrors["FechaNacimiento"] = "La fecha de nacimiento es obligatoria.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(
							kv => kv.Key,
							kv => new[] { kv.Value }));
				}

				// ── Persistencia ─────────────────────────────────────────────
				var paciente = new Paciente();
				vm.MapViewModelToPaciente(paciente);
				paciente.DomicilioNumero = vm.DomicilioNumero;
				paciente.Telefono = vm.Telefono;
				_unitOfWork.PacienteRepository.Add(paciente);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
			}
			catch (Exception ex)
			{
				// FromException traduce DbUpdateException (DNI duplicado,
				// FK violation, NULL en columna obligatoria, etc.) a
				// mensajes amigables en español.
				return ApiErrorResponse.FromException(ex, "Paciente");
			}
		}

		// PUT api/paciente/Update/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] PacienteViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (string.IsNullOrWhiteSpace(vm.Nombre))
					fieldErrors["Nombre"] = "El nombre es obligatorio.";
				if (string.IsNullOrWhiteSpace(vm.Apellido))
					fieldErrors["Apellido"] = "El apellido es obligatorio.";
				if (string.IsNullOrWhiteSpace(vm.Dni))
					fieldErrors["Dni"] = "El DNI es obligatorio.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(
							kv => kv.Key,
							kv => new[] { kv.Value }));
				}

				var paciente = _unitOfWork.PacienteRepository.GetById(id);
				if (paciente == null) return ApiErrorResponse.NotFound("Paciente no encontrado.");

				vm.Id = id;
				vm.MapViewModelToPaciente(paciente);
				paciente.DomicilioNumero = vm.DomicilioNumero;
				paciente.Telefono = vm.Telefono;
				_unitOfWork.PacienteRepository.Upsert(paciente);
				_unitOfWork.Save();
				return Ok(paciente);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Paciente");
			}
		}

		// DELETE api/paciente/Delete/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.PacienteRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				// Si el paciente tiene turnos / pagos / historias clínicas
				// asociadas, FromException devuelve
				// "No se puede eliminar el registro porque tiene datos asociados."
				return ApiErrorResponse.FromException(ex, "Paciente");
			}
		}

		// GET api/paciente/GetAllByNombreDni?searchText=juan
		[HttpGet("GetAllByNombreDni")]
		public IActionResult GetAllByNombreDni([FromQuery] string searchText)
		{
			try
			{
				var lista = _unitOfWork.StoredProcedureRepository.GetPacienteForTurno(searchText).ToList();
				return Ok(lista);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/paciente/GetNombreById/5
		[HttpGet("GetNombreById/{id}")]
		public IActionResult GetNombreById(int id)
		{
			try
			{
				var paciente = _unitOfWork.PacienteRepository.GetById(id);
				if (paciente == null) return ApiErrorResponse.NotFound("Paciente no encontrado.");
				return Ok(new { NombreCompleto = paciente.Apellido + ", " + paciente.Nombre });
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}
	}
}
