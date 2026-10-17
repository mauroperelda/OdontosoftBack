using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Services;
using OdontoSoft.Services.Common;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class TratamientoController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITratamientoServices _tratamientoServices;

    public TratamientoController(IUnitOfWork unitOfWork, ITratamientoServices tratamientoServices)
    {
      _unitOfWork = unitOfWork;
      _tratamientoServices = tratamientoServices;
    }

    // GET api/tratamiento
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Tratamiento>> GetAll()
    {
      var tratamientos = _unitOfWork.TratamientoRepository.GetAllEnabled().ToList();
      return Ok(tratamientos);
    }

    // GET api/tratamiento/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Tratamiento> GetById(int id)
    {
      var tratamiento = _unitOfWork.TratamientoRepository.GetById(id);
      if (tratamiento == null) return ApiErrorResponse.NotFound("Tratamiento no encontrado.");
      return Ok(tratamiento);
    }

    // POST api/tratamiento
    [HttpPost("Create")]
    public ActionResult<Tratamiento> Create([FromBody] Tratamiento tratamiento)
    {
      _unitOfWork.TratamientoRepository.Add(tratamiento);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = tratamiento.Id }, tratamiento);
    }

    // PUT api/tratamiento/5
    [HttpPut("Update/{id}")]
    public ActionResult<Tratamiento> Update(int id, [FromBody] Tratamiento tratamiento)
    {
      var existente = _unitOfWork.TratamientoRepository.GetById(id);
      if (existente == null) return ApiErrorResponse.NotFound("Tratamiento no encontrado.");
      tratamiento.Id = id;
      _unitOfWork.TratamientoRepository.Upsert(tratamiento);
      _unitOfWork.Save();
      return Ok(tratamiento);
    }

    // DELETE api/tratamiento/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.TratamientoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // GET api/tratamiento/GetAllConPrecios
    [HttpGet("GetAllConPrecios")]
    public ActionResult<IEnumerable<TratamientoConPreciosDto>> GetAllConPrecios()
    {
      return Ok(_tratamientoServices.GetAllConPrecios());
    }

    // PUT api/tratamiento/{tratamientoId}/Precio
    [HttpPut("{tratamientoId}/Precio")]
    public IActionResult UpsertPrecio(int tratamientoId, [FromBody] UpsertPrecioRequest req)
    {
      if (req == null)
        return ApiErrorResponse.BadRequest("Falta el body.");

      var result = _tratamientoServices.UpsertPrecio(tratamientoId, req.UbicacionId, req.Precio);
      if (!result.IsSuccess)
      {
        if (result.Error?.Contains("no encontrad") == true)
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }
      return Ok(new { TratamientoId = tratamientoId, UbicacionId = req.UbicacionId, Precio = req.Precio });
    }

    // DELETE api/tratamiento/{tratamientoId}/Precio/{ubicacionId}
    [HttpDelete("{tratamientoId}/Precio/{ubicacionId}")]
    public IActionResult DeletePrecio(int tratamientoId, int ubicacionId)
    {
      var result = _tratamientoServices.DeletePrecio(tratamientoId, ubicacionId);
      if (!result.IsSuccess) return ApiErrorResponse.NotFound(result.Error);
      return NoContent();
    }

    private ActionResult BadRequest(ServiceResult result)
    {
      var fieldErrors = result.FieldErrors?
        .ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
      return ApiErrorResponse.BadRequest(result.Error ?? "Solicitud inválida.", fieldErrors);
    }
  }

  // Body para upsert de precio
  public class UpsertPrecioRequest
  {
    public int UbicacionId { get; set; }
    public decimal Precio { get; set; }
  }
}
