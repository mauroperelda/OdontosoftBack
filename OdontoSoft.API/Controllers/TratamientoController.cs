using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;
using System.Linq;

namespace OdontoSoft.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class TratamientoController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public TratamientoController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET api/tratamiento
		[HttpGet("GetAll")]
		public IActionResult GetAll()
		{
			try
			{
				var tratamientos = _unitOfWork.TratamientoRepository.GetAllEnabled().ToList();
				return Ok(tratamientos);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// GET api/tratamiento/5
		[HttpGet("GetById/{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var tratamiento = _unitOfWork.TratamientoRepository.GetById(id);
				if (tratamiento == null) return ApiErrorResponse.NotFound("Tratamiento no encontrado.");
				return Ok(tratamiento);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex);
			}
		}

		// POST api/tratamiento
		[HttpPost("Create")]
		public IActionResult Create([FromBody] Tratamiento tratamiento)
		{
			try
			{
				_unitOfWork.TratamientoRepository.Add(tratamiento);
				_unitOfWork.Save();
				return CreatedAtAction(nameof(GetById), new { id = tratamiento.Id }, tratamiento);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Tratamiento");
			}
		}

		// PUT api/tratamiento/5
		[HttpPut("Update/{id}")]
		public IActionResult Update(int id, [FromBody] Tratamiento tratamiento)
		{
			try
			{
				var existente = _unitOfWork.TratamientoRepository.GetById(id);
				if (existente == null) return ApiErrorResponse.NotFound("Tratamiento no encontrado.");
				tratamiento.Id = id;
				_unitOfWork.TratamientoRepository.Upsert(tratamiento);
				_unitOfWork.Save();
				return Ok(tratamiento);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Tratamiento");
			}
		}

		// DELETE api/tratamiento/5
		[HttpDelete("Delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				_unitOfWork.TratamientoRepository.Delete(id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Tratamiento");
			}
		}

		// GET api/tratamiento/GetAllConPrecios
		// Devuelve los tratamientos con sus precios por ubicación anidados.
		// Es el endpoint que consume la pestaña "Servicios" de Ajustes.
		[HttpGet("GetAllConPrecios")]
		public IActionResult GetAllConPrecios()
		{
			try
			{
				var tratamientos = _unitOfWork.TratamientoRepository.GetAllEnabled().ToList();
				var ubicaciones = _unitOfWork.UbicacionRepository.GetAllEnabled().ToList();
				var precios = _unitOfWork.TratamientoPrecioRepository
					.All()
					.ToList();

				var vms = tratamientos.Select(t => new TratamientoConPreciosViewModel
				{
					Id = t.Id,
					Codigo = t.Codigo,
					Nombre = t.Nombre,
					Descripcion = t.Descripcion,
					Disabled = t.Disabled,
					Precios = precios
						.Where(p => p.TratamientoId == t.Id)
						.Select(p => new PrecioPorUbicacion
						{
							UbicacionId = p.UbicacionId,
							UbicacionNombre = ubicaciones.FirstOrDefault(u => u.Id == p.UbicacionId)?.Nombre ?? "",
							Precio = p.Precio,
						})
						.ToList(),
				}).ToList();

				return Ok(vms);
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "Tratamiento");
			}
		}

		// PUT api/tratamiento/{tratamientoId}/Precio
		// Body: { ubicacionId, precio } → upsert del precio para esa combinación.
		[HttpPut("{tratamientoId}/Precio")]
		public IActionResult UpsertPrecio(int tratamientoId, [FromBody] UpsertPrecioRequest req)
		{
			try
			{
				if (req == null) return ApiErrorResponse.BadRequest("Falta el body.");
				if (req.UbicacionId <= 0) return ApiErrorResponse.BadRequest("UbicacionId es obligatorio.");
				if (req.Precio < 0) return ApiErrorResponse.BadRequest("El precio no puede ser negativo.");

				var t = _unitOfWork.TratamientoRepository.GetById(tratamientoId);
				if (t == null) return ApiErrorResponse.NotFound("Tratamiento no encontrado.");

				var u = _unitOfWork.UbicacionRepository.GetById(req.UbicacionId);
				if (u == null) return ApiErrorResponse.NotFound("Ubicación no encontrada.");

				// Buscar existente (único por tratamiento+ubicacion).
				var existente = _unitOfWork.TratamientoPrecioRepository
					.All()
					.FirstOrDefault(p => p.TratamientoId == tratamientoId && p.UbicacionId == req.UbicacionId);

				if (existente == null)
				{
					var nuevo = new TratamientoPrecio
					{
						TratamientoId = tratamientoId,
						UbicacionId = req.UbicacionId,
						Precio = req.Precio,
					};
					_unitOfWork.TratamientoPrecioRepository.Add(nuevo);
				}
				else
				{
					existente.Precio = req.Precio;
					_unitOfWork.TratamientoPrecioRepository.Upsert(existente);
				}

				_unitOfWork.Save();
				return Ok(new { TratamientoId = tratamientoId, UbicacionId = req.UbicacionId, Precio = req.Precio });
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "TratamientoPrecio");
			}
		}

		// DELETE api/tratamiento/{tratamientoId}/Precio/{ubicacionId}
		[HttpDelete("{tratamientoId}/Precio/{ubicacionId}")]
		public IActionResult DeletePrecio(int tratamientoId, int ubicacionId)
		{
			try
			{
				var existente = _unitOfWork.TratamientoPrecioRepository
					.All()
					.FirstOrDefault(p => p.TratamientoId == tratamientoId && p.UbicacionId == ubicacionId);
				if (existente == null) return ApiErrorResponse.NotFound("Precio no encontrado.");

				_unitOfWork.TratamientoPrecioRepository.Delete(existente.Id);
				_unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				return ApiErrorResponse.FromException(ex, "TratamientoPrecio");
			}
		}
	}

	// Body para upsert de precio
	public class UpsertPrecioRequest
	{
		public int UbicacionId { get; set; }
		public decimal Precio { get; set; }
	}
}
