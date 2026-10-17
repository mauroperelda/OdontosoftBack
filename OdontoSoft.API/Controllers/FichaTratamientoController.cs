using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class FichaTratamientoController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public FichaTratamientoController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    // GET api/fichatratamiento
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<FichaTratamiento>> GetAll()
    {
      var fichas = _unitOfWork.FichaTratamientoRepository.GetAllEnabled().ToList();
      return Ok(fichas);
    }

    // GET api/fichatratamiento/5
    [HttpGet("GetById/{id}")]
    public ActionResult<FichaTratamiento> GetById(int id)
    {
      var ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(id);
      if (ficha == null) return ApiErrorResponse.NotFound("Ficha de tratamiento no encontrada.");
      return Ok(ficha);
    }

    // POST api/fichatratamiento
    [HttpPost("Create")]
    public ActionResult<FichaTratamiento> Create([FromBody] FichaTratamientoDto vm)
    {
      var ficha = new FichaTratamiento();
      ficha.FechaRealizacion = DateTime.Now;
      vm.MapDtoToFichaTratamiento(ficha);
      _unitOfWork.FichaTratamientoRepository.Add(ficha);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = ficha.Id }, ficha);
    }

    // PUT api/fichatratamiento/5
    [HttpPut("Update/{id}")]
    public ActionResult<FichaTratamiento> Update(int id, [FromBody] FichaTratamientoDto vm)
    {
      var ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(id);
      if (ficha == null) return ApiErrorResponse.NotFound("Ficha de tratamiento no encontrada.");
      vm.Id = id;
      vm.MapDtoToFichaTratamiento(ficha);
      _unitOfWork.FichaTratamientoRepository.Upsert(ficha);
      _unitOfWork.Save();
      return Ok(ficha);
    }

    // DELETE api/fichatratamiento/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.FichaTratamientoRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }
  }
}
