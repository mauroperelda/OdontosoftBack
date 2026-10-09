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
	public class EstadoTurnoController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public EstadoTurnoController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/estadoturno
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var estados = _unitOfWork.EstadoTurnoRepository.GetAllEnabled().ToList();
				return Ok(estados);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/estadoturno/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var estado = _unitOfWork.EstadoTurnoRepository.GetById(id);
				if (estado == null) return ApiErrorResponse.NotFound("Estado de turno no encontrado.");
				return Ok(estado);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/estadoturno
		[HttpPost("Create")]
		public IActionResult Create([FromBody] EstadoTurno estadoTurno)
		{
			try
			{
				_unitOfWork.EstadoTurnoRepository.Add(estadoTurno);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = estadoTurno.Id }, estadoTurno);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "EstadoTurno");
			}
		}

		// PUT api/estadoturno/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] EstadoTurno estadoTurno)
		{
			try
			{
				var existente = _unitOfWork.EstadoTurnoRepository.GetById(id);
				if (existente == null) return ApiErrorResponse.NotFound("Estado de turno no encontrado.");
				estadoTurno.Id = id;
				_unitOfWork.EstadoTurnoRepository.Upsert(estadoTurno);
				_unitOfWork.Save();
				return Ok(estadoTurno);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "EstadoTurno");
			}
		}

		// DELETE api/estadoturno/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.EstadoTurnoRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "EstadoTurno");
			}
		}
	}
}
