using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;
using System.Text.Json.Serialization;

namespace OdontoSoft.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class TurnoController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public TurnoController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/turno/GetAll?page=1&pageSize=50&searchText=
		[HttpGet("GetAll")]
		public IActionResult GetAll([FromQuery] PaginationParams pagination)
		{
			try
			{
				var query = _unitOfWork.TurnoRepository.GetAll();
				var total = query.Count();
				var items = query
					.Skip((pagination.Page - 1) * pagination.PageSize)
					.Take(pagination.PageSize)
					.ToList();
				return Ok(new PagedResult<Turno>
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

		// GET api/turno/GetById/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var turno = _unitOfWork.TurnoRepository.GetByTurnoId(id);
				if (turno == null) return ApiErrorResponse.NotFound("Turno no encontrado.");
				return Ok(turno);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/turno/GetCalendario
		// Devuelve los turnos del mes con datos expandidos. No pagina porque se usa
		// para pintar el calendario; si la cantidad crece, mover a paginación server-side.
		[HttpGet("GetCalendario")]
		public IActionResult GetCalendario()
		{
			try
			{
				// Incluimos Ubicacion para que el front pueda filtrar por consultorio.
				var turnos = _unitOfWork.TurnoRepository
					.GetAll()
					.ToList();
				var viewModels = turnos.Select(item => new TurnoViewModel
				{
					Id = item.Id,
					PacienteId = item.PacienteId,
					PacienteObj = item.Paciente,
					Paciente = item.Paciente?.Apellido + ", " + item.Paciente?.Nombre,
					ProfesionalId = item.ProfesionalId,
					ProfesionalObj = item.Profesional,
					Profesional = item.Profesional?.Apellido + ", " + item.Profesional?.Nombre,
					Fecha = item.Fecha,
					HoraInicio = item.HoraInicio,
					HoraFin = item.HoraFin,
					Observacion = item.Observacion,
					TratamientoId = item.TratamientoId,
					TratamientoObj = item.Tratamiento,
					EstadoTurnoId = item.EstadoTurnoId,
					EstadoTurnoObj = item.EstadoTurno,
					Dni = item.Paciente?.Dni,
					ColorEstado = item.EstadoTurno?.ColorHex ?? "#007bff",
					UbicacionId = item.UbicacionId,
					UbicacionObj = item.Ubicacion,
					Ubicacion = item.Ubicacion?.Nombre,
				}).ToList();
				Console.WriteLine($"Turnos obtenidos para calendario: {JsonConvert.SerializeObject(viewModels)}");
				return Ok(viewModels);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/turno/Create
		[HttpPost("Create")]
		public IActionResult Create([FromBody] TurnoViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (vm.PacienteId == 0)
					fieldErrors["PacienteId"] = "El paciente es obligatorio.";
				if (vm.ProfesionalId == 0)
					fieldErrors["ProfesionalId"] = "El profesional es obligatorio.";
				if (vm.Fecha == null)
					fieldErrors["Fecha"] = "La fecha es obligatoria.";
				if (vm.HoraInicio == null)
					fieldErrors["HoraInicio"] = "La hora de inicio es obligatoria.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				var conflicto = _unitOfWork.TurnoRepository.GetByProfesionalByHoraInicio(vm.ProfesionalId, vm.HoraInicio);
				if (conflicto != null)
				{
					return ApiErrorResponse.Conflict(
						"El profesional ya tiene un turno agendado en esa hora.",
						new Dictionary<string, string[]>
						{
							["HoraInicio"] = new[] { "Elegí otro horario para este profesional." }
						});
				}

				var turno = new Turno();
				vm.MapViewModelToTurno(turno);
				_unitOfWork.TurnoRepository.Add(turno);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = turno.Id }, turno);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Turno");
			}
		}

		// PUT api/turno/Update/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] TurnoViewModel vm)
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

				var turno = _unitOfWork.TurnoRepository.GetByTurnoId(id);
				if (turno == null) return ApiErrorResponse.NotFound("Turno no encontrado.");

				vm.Id = id;
				vm.MapViewModelToTurno(turno);
				_unitOfWork.TurnoRepository.Upsert(turno);
				_unitOfWork.Save();
				return Ok(turno);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Turno");
			}
		}

		// DELETE api/turno/Delete/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.TurnoRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Turno");
			}
		}
	}
}
