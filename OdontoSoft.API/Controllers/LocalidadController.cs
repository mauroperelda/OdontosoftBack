using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Application.Services;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class LocalidadController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalidadServices _localidadServices;

    public LocalidadController(IUnitOfWork unitOfWork, ILocalidadServices localidadServices)
    {
      _unitOfWork = unitOfWork;
      _localidadServices = localidadServices;
    }

    // GET api/localidad
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Localidad>> GetAll()
    {
      var localidades = _unitOfWork.LocalidadRepository.GetAllEnabled().ToList();
      return Ok(localidades);
    }

    // GET api/localidad/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Localidad> GetById(int id)
    {
      var localidad = _unitOfWork.LocalidadRepository.GetById(id);
      if (localidad == null) return ApiErrorResponse.NotFound("Localidad no encontrada.");
      return Ok(localidad);
    }

    // GET api/localidad/provincia/5
    [HttpGet("GetByProvincia/{provinciaId}")]
    public ActionResult<IEnumerable<Localidad>> GetByProvincia(int provinciaId)
    {
      var localidades = _unitOfWork.LocalidadRepository.GetByProvinciaId(provinciaId).ToList();
      return Ok(localidades);
    }

    // POST api/localidad
    [HttpPost("Create")]
    public ActionResult<Localidad> Create([FromBody] Localidad localidad)
    {
      _unitOfWork.LocalidadRepository.Add(localidad);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = localidad.Id }, localidad);
    }

    // PUT api/localidad/5
    [HttpPut("Update/{id}")]
    public ActionResult<Localidad> Update(int id, [FromBody] Localidad localidad)
    {
      var existente = _unitOfWork.LocalidadRepository.GetById(id);
      if (existente == null) return ApiErrorResponse.NotFound("Localidad no encontrada.");
      localidad.Id = id;
      _unitOfWork.LocalidadRepository.Upsert(localidad);
      _unitOfWork.Save();
      return Ok(localidad);
    }

    // DELETE api/localidad/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.LocalidadRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    /// <summary>
    /// Importa el catálogo de localidades de la Argentina.
    ///
    /// Es una operación manual: sólo se ejecuta cuando alguien hace POST a
    /// este endpoint (nunca sola, ni al arrancar la API). Toma el catálogo
    /// de la API georef de datos.gob.ar —la misma fuente que usaba el
    /// importador del proyecto Web legacy— y lo escribe en la tabla
    /// 'localidad' de esta base de datos.
    ///
    /// Es idempotente: resuelve la provincia por nombre normalizado y
    /// deduplica por (nombre, provincia), así que se puede volver a ejecutar
    /// sin duplicar nada.
    ///
    /// Con <c>?preview=true</c> no escribe nada: devuelve los contadores de
    /// lo que se insertaría, para revisar antes de escribir.
    /// </summary>
    // POST api/localidad/sincronizar
    [HttpPost("SincronizarLocalidades")]
    public async Task<ActionResult<LocalidadSyncResult>> SincronizarLocalidades([FromQuery] bool preview = false)
    {
      try
      {
        LocalidadSyncResult resultado = await _localidadServices.SincronizarLocalidadesAsync(preview);
        return Ok(resultado);
      }
      catch (Exception ex)
      {
        return ApiErrorResponse.BadRequest("No se pudo sincronizar las localidades: " + ex.Message);
      }
    }
  }
}

