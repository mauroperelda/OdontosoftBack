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
	public class FichaTratamientoController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public FichaTratamientoController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/fichatratamiento
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var fichas = _unitOfWork.FichaTratamientoRepository.GetAllEnabled().ToList();
				return Ok(fichas);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/fichatratamiento/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(id);
				if (ficha == null) return ApiErrorResponse.NotFound("Ficha de tratamiento no encontrada.");
				return Ok(ficha);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/fichatratamiento
		[HttpPost("Create")]
		public IActionResult Create([FromBody] FichaTratamientoViewModel vm)
		{
			try
			{
				var ficha = new FichaTratamiento();
				ficha.FechaRealizacion = DateTime.Now;
				vm.MapViewModelToFichaTratamiento(ficha);
				_unitOfWork.FichaTratamientoRepository.Add(ficha);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = ficha.Id }, ficha);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "FichaTratamiento");
			}
		}

		// PUT api/fichatratamiento/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] FichaTratamientoViewModel vm)
		{
			try
			{
				var ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(id);
				if (ficha == null) return ApiErrorResponse.NotFound("Ficha de tratamiento no encontrada.");
				vm.Id = id;
				vm.MapViewModelToFichaTratamiento(ficha);
				_unitOfWork.FichaTratamientoRepository.Upsert(ficha);
				_unitOfWork.Save();
				return Ok(ficha);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "FichaTratamiento");
			}
		}

		// DELETE api/fichatratamiento/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
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
	}
}
