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
  public class OdontogramaPiezaController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOdontogramaPiezaServices _odontogramaPiezaServices;

    public OdontogramaPiezaController(
      IUnitOfWork unitOfWork,
      IOdontogramaPiezaServices odontogramaPiezaServices)
    {
      _unitOfWork = unitOfWork;
      _odontogramaPiezaServices = odontogramaPiezaServices;
    }

    // GET api/odontogramapieza
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<OdontogramaPieza>> GetAll()
    {
      var piezas = _unitOfWork.OdontogramaPiezaRepository.GetAllEnabled().ToList();
      return Ok(piezas);
    }

    // GET api/odontogramapieza/5
    [HttpGet("GetById/{id}")]
    public ActionResult<OdontogramaPieza> GetById(int id)
    {
      var pieza = _unitOfWork.OdontogramaPiezaRepository.GetById(id);
      if (pieza == null) return ApiErrorResponse.NotFound("Pieza no encontrada.");
      return Ok(pieza);
    }

    // GET api/odontogramapieza/historiaclinica/5
    [HttpGet("GetByHistoriaClinica/{historiaClinicaId}")]
    public ActionResult<IEnumerable<OdontogramaPieza>> GetByHistoriaClinica(int historiaClinicaId)
    {
      var piezas = _unitOfWork.OdontogramaPiezaRepository
          .GetAllEnabled()
          .Where(p => p.HistoriaClinicaId == historiaClinicaId)
          .ToList();
      return Ok(piezas);
    }

    // POST api/odontogramapieza
    // Upsert por (HC, NumeroPieza) — la misma regla que usa HistoriaClinicaController.GuardarPieza.
    [HttpPost("Create")]
    public ActionResult<OdontogramaPieza> Create([FromBody] OdontogramaPiezaDto vm)
    {
      var result = _odontogramaPiezaServices.UpsertByPieza(vm);
      if (!result.IsSuccess) return BadRequest(result);

      // Devolvemos la pieza resultante (nueva o actualizada) para que el front
      // pueda refrescar la grilla con su id asignado.
      var existente = _unitOfWork.OdontogramaPiezaRepository
        .GetByHistoriaClinicaIdAndNumeroPieza(vm.HistoriaClinicaId, vm.NumeroPieza);
      if (existente == null) return ApiErrorResponse.NotFound("Pieza no encontrada tras upsert.");
      return Ok(existente);
    }

    // PUT api/odontogramapieza/5
    [HttpPut("Update/{id}")]
    public ActionResult<OdontogramaPieza> Update(int id, [FromBody] OdontogramaPiezaDto vm)
    {
      var pieza = _unitOfWork.OdontogramaPiezaRepository.GetById(id);
      if (pieza == null) return ApiErrorResponse.NotFound("Pieza no encontrada.");
      vm.Id = id;
      vm.MapDtoToOdontogramaPieza(pieza);
      _unitOfWork.OdontogramaPiezaRepository.Upsert(pieza);
      _unitOfWork.Save();
      return Ok(pieza);
    }

    // DELETE api/odontogramapieza/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.OdontogramaPiezaRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    private ActionResult BadRequest(ServiceResult result)
    {
      var fieldErrors = result.FieldErrors?
        .ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
      return ApiErrorResponse.BadRequest(result.Error ?? "Solicitud inválida.", fieldErrors);
    }
  }
}
