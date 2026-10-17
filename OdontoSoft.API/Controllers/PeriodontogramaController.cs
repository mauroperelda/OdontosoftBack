using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Services;
using OdontoSoft.Application.Common;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class PeriodontogramaController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPeriodontogramaServices _periodontogramaServices;

    public PeriodontogramaController(
      IUnitOfWork unitOfWork,
      IPeriodontogramaServices periodontogramaServices)
    {
      _unitOfWork = unitOfWork;
      _periodontogramaServices = periodontogramaServices;
    }

    // GET api/periodontograma
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<PeriodontogramaDto>> GetAll()
    {
      var items = _unitOfWork.PeriodontogramaRepository
        .GetAllEnabled()
        .OrderByDescending(x => x.FechaRegistro)
        .ToList();
      return Ok(items.Select(p => new PeriodontogramaDto(p)).ToList());
    }

    // GET api/periodontograma/5
    [HttpGet("GetById/{id}")]
    public ActionResult<PeriodontogramaDto> GetById(int id)
    {
      var p = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
      if (p == null) return ApiErrorResponse.NotFound("Periodontograma no encontrado.");
      var vm = new PeriodontogramaDto(p);
      vm.Metricas = _periodontogramaServices.CalcularMetricas(p.Piezas);
      return Ok(vm);
    }

    // GET api/periodontograma/historiaclinica/5
    [HttpGet("GetByHistoriaClinica/{historiaClinicaId}")]
    public ActionResult<PeriodontogramaDto?> GetByHistoriaClinica(int historiaClinicaId)
    {
      var p = _unitOfWork.PeriodontogramaRepository.GetLastByHistoriaClinicaId(historiaClinicaId);
      if (p == null) return Ok(null);
      var vm = new PeriodontogramaDto(p);
      vm.Metricas = _periodontogramaServices.CalcularMetricas(p.Piezas);
      return Ok(vm);
    }

    // GET api/periodontograma/metricas/5
    [HttpGet("GetMetricas/{id}")]
    public ActionResult<PeriodontogramaMetricasDto> GetMetricas(int id)
    {
      var p = _unitOfWork.PeriodontogramaRepository.GetWithPiezasById(id);
      if (p == null) return ApiErrorResponse.NotFound("Periodontograma no encontrado.");
      return Ok(_periodontogramaServices.CalcularMetricas(p.Piezas));
    }

    // POST api/periodontograma
    [HttpPost("Create")]
    public ActionResult<PeriodontogramaDto> Create([FromBody] PeriodontogramaDto vm)
    {
      var result = _periodontogramaServices.Create(vm);
      if (!result.IsSuccess) return BadRequest(result);
      return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    // PUT api/periodontograma/5
    [HttpPut("Update/{id}")]
    public ActionResult<PeriodontogramaDto> Update(int id, [FromBody] PeriodontogramaDto vm)
    {
      var result = _periodontogramaServices.Update(id, vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Periodontograma no encontrado.")
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }
      return Ok(result.Value);
    }

    // DELETE api/periodontograma/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      var result = _periodontogramaServices.Delete(id);
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
}
