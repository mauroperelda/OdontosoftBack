using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Contracts.Common;
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
  public class TurnoController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TurnoController> _logger;
    private readonly ITurnoServices _turnoServices;

    public TurnoController(IUnitOfWork unitOfWork, ILogger<TurnoController> logger, ITurnoServices turnoServices)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
      _turnoServices = turnoServices;
    }

    // GET api/turno/GetAll?page=1&pageSize=50&searchText=
    [HttpGet("GetAll")]
    public async Task<ActionResult<PagedResult<Turno>>> GetAll([FromQuery] PaginationParams pagination)
    {
      var result = await _unitOfWork.TurnoRepository
        .GetAll()
        .ToPagedResultAsync(pagination);
      return Ok(result);
    }

    // GET api/turno/GetById/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Turno> GetById(int id)
    {
      var turno = _unitOfWork.TurnoRepository.GetByTurnoId(id);
      if (turno == null) return ApiErrorResponse.NotFound("Turno no encontrado.");
      return Ok(turno);
    }

    // GET api/turno/GetCalendario
    // Devuelve los turnos del mes con datos expandidos. No pagina porque se usa
    // para pintar el calendario; si la cantidad crece, mover a paginación server-side.
    [HttpGet("GetCalendario")]
    public ActionResult<IEnumerable<TurnoDto>> GetCalendario()
    {
      // Incluimos Ubicacion para que el front pueda filtrar por consultorio.
      var turnos = _unitOfWork.TurnoRepository
        .GetAll()
        .ToList();
      var viewModels = turnos.Select(item => new TurnoDto
      {
        Id = item.Id,
        PacienteId = item.PacienteId,
        PacienteObj = item.Paciente,
        Paciente = item.Paciente?.Apellido + ", " + item.Paciente?.Nombre,
        ProfesionalId = item.ProfesionalId,
        ProfesionalObj = item.Profesional,
        Profesional = item.Profesional?.Apellido + ", " + item.Profesional?.Nombre,
        Fecha = item.Fecha,
        HoraInicio = item.HoraInicio,
        HoraFin = item.HoraFin,
        Observacion = item.Observacion,
        TratamientoId = item.TratamientoId,
        TratamientoObj = item.Tratamiento,
        EstadoTurnoId = item.EstadoTurnoId,
        EstadoTurnoObj = item.EstadoTurno,
        Dni = item.Paciente?.Dni,
        ColorEstado = item.EstadoTurno?.ColorHex ?? "#007bff",
        UbicacionId = item.UbicacionId,
        UbicacionObj = item.Ubicacion,
        Ubicacion = item.Ubicacion?.Nombre,
      }).ToList();
      _logger.LogDebug("Turnos del calendario: {Count} turnos", viewModels.Count);
      return Ok(viewModels);
    }

    // POST api/turno/Create
    // Validación de shape delegada a TurnoDtoValidator.
    // Validación de conflicto de horario delegada a ITurnoServices.
    [HttpPost("Create")]
    public ActionResult<Turno> Create([FromBody] TurnoDto vm)
    {
      var result = _turnoServices.Create(vm);
      if (!result.IsSuccess) return Conflict(result);
      var turno = _unitOfWork.TurnoRepository.GetByTurnoId(result.Id!.Value);
      return CreatedAtAction(nameof(GetById), new { id = result.Id }, turno);
    }

    // PUT api/turno/Update/5
    [HttpPut("Update/{id}")]
    public ActionResult<Turno> Update(int id, [FromBody] TurnoDto vm)
    {
      var result = _turnoServices.Update(id, vm);
      if (!result.IsSuccess)
      {
        if (result.Error == "Turno no encontrado.")
          return ApiErrorResponse.NotFound(result.Error);
        return Conflict(result);
      }
      var turno = _unitOfWork.TurnoRepository.GetByTurnoId(id);
      return Ok(turno);
    }

    // DELETE api/turno/Delete/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.TurnoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // Traduce un ServiceResult fallido a IActionResult 409 (conflicto de horario).
    private ActionResult Conflict(ServiceResult result)
    {
      var fieldErrors = result.FieldErrors?
        .ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
      return ApiErrorResponse.Conflict(result.Error ?? "Conflicto de horario.", fieldErrors);
    }
  }
}

