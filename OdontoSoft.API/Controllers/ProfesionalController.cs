using AutoMapper;
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
  public class ProfesionalController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProfesionalController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    // GET api/profesional
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<ProfesionalDto>> GetAll()
    {
      var profesionales = _unitOfWork.ProfesionalRepository.GetAllEnabled().ToList();
      return Ok(_mapper.Map<List<ProfesionalDto>>(profesionales));
    }

    // GET api/profesional/5
    [HttpGet("GetById/{id}")]
    public ActionResult<ProfesionalDto> GetById(int id)
    {
      var profesional = _unitOfWork.ProfesionalRepository.GetById(id);
      if (profesional == null) return ApiErrorResponse.NotFound("Profesional no encontrado.");
      return Ok(_mapper.Map<ProfesionalDto>(profesional));
    }

    // POST api/profesional
    // Validación de shape delegada a ProfesionalDtoValidator (FluentValidation).
    [HttpPost("Create")]
    public ActionResult<ProfesionalDto> Create([FromBody] ProfesionalDto vm)
    {
      var profesional = _mapper.Map<Profesional>(vm);
      _unitOfWork.ProfesionalRepository.Add(profesional);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = profesional.Id }, _mapper.Map<ProfesionalDto>(profesional));
    }

    // PUT api/profesional/5
    [HttpPut("Update/{id}")]
    public ActionResult<ProfesionalDto> Update(int id, [FromBody] ProfesionalDto vm)
    {
      var profesional = _unitOfWork.ProfesionalRepository.GetById(id);
      if (profesional == null) return ApiErrorResponse.NotFound("Profesional no encontrado.");

      vm.Id = id;
      _mapper.Map(vm, profesional);
      _unitOfWork.ProfesionalRepository.Upsert(profesional);
      _unitOfWork.Save();
      return Ok(_mapper.Map<ProfesionalDto>(profesional));
    }

    // DELETE api/profesional/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.ProfesionalRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }
  }
}
