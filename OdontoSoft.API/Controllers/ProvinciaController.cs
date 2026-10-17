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
  public class ProvinciaController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public ProvinciaController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    // GET api/provincia
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Provincia>> GetAll()
    {
      var provincias = _unitOfWork.ProvinciaRepository.GetAllEnabled().ToList();
      return Ok(provincias);
    }

    // GET api/provincia/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Provincia> GetById(int id)
    {
      var provincia = _unitOfWork.ProvinciaRepository.GetById(id);
      if (provincia == null) return ApiErrorResponse.NotFound("Provincia no encontrada.");
      return Ok(provincia);
    }

    // POST api/provincia
    [HttpPost("Create")]
    public ActionResult<Provincia> Create([FromBody] Provincia provincia)
    {
      _unitOfWork.ProvinciaRepository.Add(provincia);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = provincia.Id }, provincia);
    }

    // PUT api/provincia/5
    [HttpPut("Update/{id}")]
    public ActionResult<Provincia> Update(int id, [FromBody] Provincia provincia)
    {
      var existente = _unitOfWork.ProvinciaRepository.GetById(id);
      if (existente == null) return ApiErrorResponse.NotFound("Provincia no encontrada.");
      provincia.Id = id;
      _unitOfWork.ProvinciaRepository.Upsert(provincia);
      _unitOfWork.Save();
      return Ok(provincia);
    }

    // DELETE api/provincia/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.ProvinciaRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }
  }
}

