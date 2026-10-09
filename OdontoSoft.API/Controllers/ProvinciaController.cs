using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;

namespace OdontoSoft.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class ProvinciaController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProvinciaController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/provincia
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var provincias = _unitOfWork.ProvinciaRepository.GetAllEnabled().ToList();
				return Ok(provincias);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/provincia/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var provincia = _unitOfWork.ProvinciaRepository.GetById(id);
				if (provincia == null) return ApiErrorResponse.NotFound("Provincia no encontrada.");
				return Ok(provincia);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/provincia
		[HttpPost("Create")]
		public IActionResult Create([FromBody] Provincia provincia)
		{
			try
			{
				_unitOfWork.ProvinciaRepository.Add(provincia);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = provincia.Id }, provincia);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Provincia");
			}
		}

		// PUT api/provincia/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] Provincia provincia)
		{
			try
			{
				var existente = _unitOfWork.ProvinciaRepository.GetById(id);
				if (existente == null) return ApiErrorResponse.NotFound("Provincia no encontrada.");
				provincia.Id = id;
				_unitOfWork.ProvinciaRepository.Upsert(provincia);
				_unitOfWork.Save();
				return Ok(provincia);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Provincia");
			}
		}

		// DELETE api/provincia/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.ProvinciaRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Provincia");
			}
		}
	}
}
