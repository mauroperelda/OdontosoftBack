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
	public class PresupuestoDetalleController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public PresupuestoDetalleController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/presupuestodetalle
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var detalles = _unitOfWork.PresupuestoDetalleRepository.GetAllEnabled().ToList();
				return Ok(detalles);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/presupuestodetalle/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
				if (detalle == null) return ApiErrorResponse.NotFound("Detalle no encontrado.");
				return Ok(detalle);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/presupuestodetalle
		[HttpPost("Create")]
		public IActionResult Create([FromBody] PresupuestoDetalleViewModel vm)
		{
			try
			{
				var detalle = new PresupuestoDetalle();
				vm.MapViewModelToDetalle(detalle);
				_unitOfWork.PresupuestoDetalleRepository.Add(detalle);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, detalle);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "PresupuestoDetalle");
			}
		}

		// PUT api/presupuestodetalle/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] PresupuestoDetalleViewModel vm)
		{
			try
			{
				var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
				if (detalle == null) return ApiErrorResponse.NotFound("Detalle no encontrado.");
				vm.Id = id;
				vm.MapViewModelToDetalle(detalle);
				_unitOfWork.PresupuestoDetalleRepository.Upsert(detalle);
				_unitOfWork.Save();
				return Ok(detalle);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "PresupuestoDetalle");
			}
		}

		// DELETE api/presupuestodetalle/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.PresupuestoDetalleRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "PresupuestoDetalle");
			}
		}
	}
}
