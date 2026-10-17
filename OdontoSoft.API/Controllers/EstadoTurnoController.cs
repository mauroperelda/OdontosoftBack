using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;

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
    public ActionResult<IEnumerable<EstadoTurno>> GetAll()
    {
      var estados = _unitOfWork.EstadoTurnoRepository.GetAllEnabled().ToList();
      return Ok(estados);
    }

    // GET api/estadoturno/5
    [HttpGet("GetById/{id}")]
    public ActionResult<EstadoTurno> GetById(int id)
    {
      var estado = _unitOfWork.EstadoTurnoRepository.GetById(id);
      if (estado == null) return ApiErrorResponse.NotFound("Estado de turno no encontrado.");
      return Ok(estado);
    }

    // POST api/estadoturno
    [HttpPost("Create")]
    public ActionResult<EstadoTurno> Create([FromBody] EstadoTurno estadoTurno)
    {
      _unitOfWork.EstadoTurnoRepository.Add(estadoTurno);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = estadoTurno.Id }, estadoTurno);
    }

    // PUT api/estadoturno/5
    [HttpPut("Update/{id}")]
    public ActionResult<EstadoTurno> Update(int id, [FromBody] EstadoTurno estadoTurno)
    {
      var existente = _unitOfWork.EstadoTurnoRepository.GetById(id);
      if (existente == null) return ApiErrorResponse.NotFound("Estado de turno no encontrado.");
      estadoTurno.Id = id;
      _unitOfWork.EstadoTurnoRepository.Upsert(estadoTurno);
      _unitOfWork.Save();
      return Ok(estadoTurno);
    }

    // DELETE api/estadoturno/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.EstadoTurnoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }
  }
}

