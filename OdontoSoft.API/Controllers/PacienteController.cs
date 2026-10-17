using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Contracts.Common;
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
  public class PacienteController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPacienteServices _pacienteServices;

    public PacienteController(IUnitOfWork unitOfWork, IMapper mapper, IPacienteServices pacienteServices)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _pacienteServices = pacienteServices;
    }

    // GET api/paciente/GetAll?page=1&pageSize=50
    [HttpGet("GetAll")]
    public async Task<ActionResult<PagedResult<PacienteDto>>> GetAll([FromQuery] PaginationParams pagination)
    {
      var result = await _unitOfWork.PacienteRepository
        .GetAllEnabled()
        .ToPagedResultAsync(pagination);
      // Proyectamos a VM para que el front reciba DTOs consistentes con
      // los que recibe en POST/PUT (sin auditar campos como CreatedDate).
      var vms = _mapper.Map<List<PacienteDto>>(result.Items);
      return Ok(new PagedResult<PacienteDto>
      {
        Items = vms,
        Total = result.Total,
        Page = result.Page,
        PageSize = result.PageSize,
      });
    }

    // GET api/paciente/GetById/5
    [HttpGet("GetById/{id}")]
    public ActionResult<PacienteDto> GetById(int id)
    {
      var paciente = _unitOfWork.PacienteRepository.GetById(id);
      if (paciente == null) return ApiErrorResponse.NotFound("Paciente no encontrado.");
      return Ok(_mapper.Map<PacienteDto>(paciente));
    }

    // POST api/paciente/Create
    // Validación de shape delegada a PacienteDtoValidator (FluentValidation).
    // Validación de unicidad de DNI delegada a IPacienteServices.
    [HttpPost("Create")]
    public async Task<ActionResult<PacienteDto>> Create([FromBody] PacienteDto vm)
    {
      var dniCheck = await _pacienteServices.ValidarDniUnicoAsync(vm.Dni);
      if (!dniCheck.IsSuccess) return BadRequest(dniCheck);

      var paciente = _mapper.Map<Paciente>(vm);
      _unitOfWork.PacienteRepository.Add(paciente);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, _mapper.Map<PacienteDto>(paciente));
    }

    // PUT api/paciente/Update/5
    [HttpPut("Update/{id}")]
    public async Task<ActionResult<PacienteDto>> Update(int id, [FromBody] PacienteDto vm)
    {
      var paciente = _unitOfWork.PacienteRepository.GetById(id);
      if (paciente == null) return ApiErrorResponse.NotFound("Paciente no encontrado.");

      var dniCheck = await _pacienteServices.ValidarDniUnicoAsync(vm.Dni, excludePacienteId: id);
      if (!dniCheck.IsSuccess) return BadRequest(dniCheck);

      vm.Id = id;
      _mapper.Map(vm, paciente);
      _unitOfWork.PacienteRepository.Upsert(paciente);
      _unitOfWork.Save();
      return Ok(_mapper.Map<PacienteDto>(paciente));
    }

    // DELETE api/paciente/Delete/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.PacienteRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // GET api/paciente/GetAllByNombreDni?searchText=juan
    [HttpGet("GetAllByNombreDni")]
    public ActionResult<IEnumerable<object>> GetAllByNombreDni([FromQuery] string searchText)
    {
      var lista = _unitOfWork.StoredProcedureRepository.GetPacienteForTurno(searchText).ToList();
      return Ok(lista);
    }

    // GET api/paciente/GetNombreById/5
    [HttpGet("GetNombreById/{id}")]
    public ActionResult<object> GetNombreById(int id)
    {
      var paciente = _unitOfWork.PacienteRepository.GetById(id);
      if (paciente == null) return ApiErrorResponse.NotFound("Paciente no encontrado.");
      return Ok(new { NombreCompleto = paciente.Apellido + ", " + paciente.Nombre });
    }

    // Traduce un ServiceResult fallido a IActionResult 400 con field-keyed errors.
    private ActionResult BadRequest(ServiceResult result)
    {
      var fieldErrors = result.FieldErrors?
        .ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
      return ApiErrorResponse.BadRequest(result.Error ?? "Operación inválida.", fieldErrors);
    }
  }
}
