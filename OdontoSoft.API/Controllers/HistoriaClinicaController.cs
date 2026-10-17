using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Services;
using OdontoSoft.Application.Common;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class HistoriaClinicaController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHistoriaClinicaServices _historiaClinicaServices;

    public HistoriaClinicaController(
      IUnitOfWork unitOfWork,
      IHistoriaClinicaServices historiaClinicaServices)
    {
      _unitOfWork = unitOfWork;
      _historiaClinicaServices = historiaClinicaServices;
    }

    // GET api/historiaclinica
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<HistoriaClinica>> GetAll()
    {
      var historias = _unitOfWork.HistoriaClinicaRepository.GetAllEnabled().ToList();
      return Ok(historias);
    }

    // GET api/historiaclinica/5
    [HttpGet("GetById/{id}")]
    public ActionResult<HistoriaClinica> GetById(int id)
    {
      var hc = _unitOfWork.HistoriaClinicaRepository.GetHistoriaClinicaById(id);
      if (hc == null) return ApiErrorResponse.NotFound("Historia clínica no encontrada.");
      return Ok(hc);
    }

    // GET api/historiaclinica/paciente/5
    [HttpGet("GetByPaciente/{pacienteId}")]
    public ActionResult<HistoriaClinica> GetByPaciente(int pacienteId)
    {
      var hc = _unitOfWork.HistoriaClinicaRepository.GetFullByPacienteId(pacienteId);
      if (hc == null) return ApiErrorResponse.NotFound("El paciente no tiene historia clínica.");
      return Ok(hc);
    }

    // POST api/historiaclinica
    // Validación de shape delegada a HistoriaClinicaDtoValidator.
    [HttpPost("Create")]
    public ActionResult<HistoriaClinica> Create([FromBody] HistoriaClinicaDto vm)
    {
      var result = _historiaClinicaServices.Create(vm);
      if (!result.IsSuccess) return BadRequest(result);

      var hc = _unitOfWork.HistoriaClinicaRepository.GetById(result.Id!.Value);
      return CreatedAtAction(nameof(GetById), new { id = result.Id }, hc);
    }

    // PUT api/historiaclinica/5
    [HttpPut("Update/{id}")]
    public ActionResult<HistoriaClinica> Update(int id, [FromBody] HistoriaClinicaDto vm)
    {
      var result = _historiaClinicaServices.Update(id, vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Historia clínica no encontrada.")
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }
      var hc = _unitOfWork.HistoriaClinicaRepository.GetById(id);
      return Ok(hc);
    }

    // DELETE api/historiaclinica/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.HistoriaClinicaRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // -- ODONTOGRAMA ------------------------------------------------------------

    // POST api/historiaclinica/pieza
    [HttpPost("GuardarPieza")]
    public IActionResult GuardarPieza([FromBody] OdontogramaPiezaDto vm)
    {
      var result = _historiaClinicaServices.GuardarPieza(vm);
      if (!result.IsSuccess) return BadRequest(result);
      return Ok(new { mensaje = "Pieza guardada correctamente" });
    }

    // -- FICHAS DE TRATAMIENTO -------------------------------------------------

    // POST api/historiaclinica/ficha
    [HttpPost("GuardarFicha")]
    public IActionResult GuardarFicha([FromBody] FichaTratamientoDto vm)
    {
      var result = _historiaClinicaServices.GuardarFicha(vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Ficha de tratamiento no encontrada.")
          return ApiErrorResponse.NotFound(result.Error);
        return BadRequest(result);
      }

      var ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(result.Id!.Value);
      return Ok(new
      {
        ficha!.Id,
        Tratamiento = ficha.Tratamiento?.Nombre,
        ficha.NumeroPieza,
        ficha.Descripcion,
        FechaRealizacion = ficha.FechaRealizacion.ToString("dd/MM/yyyy")
      });
    }

    // DELETE api/historiaclinica/ficha/5
    [HttpDelete("EliminarFicha/{id}")]
    public IActionResult EliminarFicha(int id)
    {
      _unitOfWork.FichaTratamientoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // GET api/historiaclinica/tratamientos
    [HttpGet("GetTratamientosParaFicha")]
    public IActionResult GetTratamientosParaFicha()
    {
      var items = _historiaClinicaServices.GetTratamientosParaFicha()
        .Select(x => new { x.Id, x.Nombre });
      return Ok(items);
    }

    // Traduce un ServiceResult fallido a 400 con field errors si los trae.
    private ActionResult BadRequest(ServiceResult result)
    {
      var fieldErrors = result.FieldErrors?
        .ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
      return ApiErrorResponse.BadRequest(result.Error ?? "Solicitud inválida.", fieldErrors);
    }
  }
}
