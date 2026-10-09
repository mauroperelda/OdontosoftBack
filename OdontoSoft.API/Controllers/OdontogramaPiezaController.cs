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
	public class OdontogramaPiezaController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public OdontogramaPiezaController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/odontogramapieza
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var piezas = _unitOfWork.OdontogramaPiezaRepository.GetAllEnabled().ToList();
				return Ok(piezas);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/odontogramapieza/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var pieza = _unitOfWork.OdontogramaPiezaRepository.GetById(id);
				if (pieza == null) return ApiErrorResponse.NotFound("Pieza no encontrada.");
				return Ok(pieza);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/odontogramapieza/historiaclinica/5
		[HttpGet("GetByHistoriaClinica/{historiaClinicaId}")]
		public IActionResult GetByHistoriaClinica(int historiaClinicaId)
		{
			try
			{
				var piezas = _unitOfWork.OdontogramaPiezaRepository
						.GetAllEnabled()
						.Where(p => p.HistoriaClinicaId == historiaClinicaId)
						.ToList();
				return Ok(piezas);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/odontogramapieza
		[HttpPost("Create")]
		public IActionResult Create([FromBody] OdontogramaPiezaViewModel vm)
		{
			try
			{
				// Upsert por historia + número de pieza (como en el Web)
				var existing = _unitOfWork.OdontogramaPiezaRepository
						.GetByHistoriaClinicaIdAndNumeroPieza(vm.HistoriaClinicaId, vm.NumeroPieza);

				if (existing != null)
				{
					vm.Id = existing.Id;
					vm.MapViewModelToOdontogramaPieza(existing);
					_unitOfWork.OdontogramaPiezaRepository.Upsert(existing);
					_unitOfWork.Save();
					return Ok(existing);
				}

				var pieza = new OdontogramaPieza();
				vm.MapViewModelToOdontogramaPieza(pieza);
				_unitOfWork.OdontogramaPiezaRepository.Add(pieza);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = pieza.Id }, pieza);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "OdontogramaPieza");
			}
		}

		// PUT api/odontogramapieza/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] OdontogramaPiezaViewModel vm)
		{
			try
			{
				var pieza = _unitOfWork.OdontogramaPiezaRepository.GetById(id);
				if (pieza == null) return ApiErrorResponse.NotFound("Pieza no encontrada.");
				vm.Id = id;
				vm.MapViewModelToOdontogramaPieza(pieza);
				_unitOfWork.OdontogramaPiezaRepository.Upsert(pieza);
				_unitOfWork.Save();
				return Ok(pieza);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "OdontogramaPieza");
			}
		}

		// DELETE api/odontogramapieza/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.OdontogramaPiezaRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "OdontogramaPieza");
			}
		}
	}
}
