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
	public class ProfesionalController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProfesionalController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/profesional
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var profesionales = _unitOfWork.ProfesionalRepository.GetAllEnabled().ToList();
				return Ok(profesionales);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/profesional/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var profesional = _unitOfWork.ProfesionalRepository.GetById(id);
				if (profesional == null) return ApiErrorResponse.NotFound("Profesional no encontrado.");
				return Ok(profesional);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/profesional
		[HttpPost("Create")]
		public IActionResult Create([FromBody] ProfesionalViewModel vm)
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
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				var profesional = new Profesional();
				vm.MapViewModelToPaciente(profesional);
				_unitOfWork.ProfesionalRepository.Add(profesional);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = profesional.Id }, profesional);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Profesional");
			}
		}

		// PUT api/profesional/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] ProfesionalViewModel vm)
		{
			try
			{
				var fieldErrors = new Dictionary<string, string>();
				if (string.IsNullOrWhiteSpace(vm.Nombre))
					fieldErrors["Nombre"] = "El nombre es obligatorio.";
				if (string.IsNullOrWhiteSpace(vm.Apellido))
					fieldErrors["Apellido"] = "El apellido es obligatorio.";

				if (fieldErrors.Count > 0)
				{
					return ApiErrorResponse.BadRequest(
						"Revisá los campos marcados.",
						fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
				}

				var profesional = _unitOfWork.ProfesionalRepository.GetById(id);
				if (profesional == null) return ApiErrorResponse.NotFound("Profesional no encontrado.");

				vm.Id = id;
				vm.MapViewModelToPaciente(profesional);
				_unitOfWork.ProfesionalRepository.Upsert(profesional);
				_unitOfWork.Save();
				return Ok(profesional);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Profesional");
			}
		}

		// DELETE api/profesional/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.ProfesionalRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Profesional");
			}
		}
	}
}
