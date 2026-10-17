using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class UbicacionController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public UbicacionController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    // GET api/ubicacion/GetAll
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<UbicacionDto>> GetAll()
    {
      var items = _unitOfWork.UbicacionRepository.GetAllEnabled().ToList();
      var vms = items.Select(MapToDto).ToList();
      return Ok(vms);
    }

    // GET api/ubicacion/GetById/5
    [HttpGet("GetById/{id}")]
    public ActionResult<UbicacionDto> GetById(int id)
    {
      var u = _unitOfWork.UbicacionRepository.GetById(id);
      if (u == null) return ApiErrorResponse.NotFound("Ubicación no encontrada.");
      return Ok(MapToDto(u));
    }

    // POST api/ubicacion/Create
    // Validación de shape delegada a UbicacionDtoValidator.
    [HttpPost("Create")]
    public ActionResult<UbicacionDto> Create([FromBody] UbicacionDto vm)
    {
      var entity = MapToEntity(vm);
      _unitOfWork.UbicacionRepository.Add(entity);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapToDto(entity));
    }

    // PUT api/ubicacion/Update/5
    [HttpPut("Update/{id}")]
    public ActionResult<UbicacionDto> Update(int id, [FromBody] UbicacionDto vm)
    {
      var existente = _unitOfWork.UbicacionRepository.GetById(id);
      if (existente == null) return ApiErrorResponse.NotFound("Ubicación no encontrada.");

      existente.Nombre = vm.Nombre;
      existente.Direccion = vm.Direccion;
      existente.CodigoPostal = vm.CodigoPostal;
      existente.Ciudad = vm.Ciudad;
      existente.Provincia = vm.Provincia;
      existente.Telefono = vm.Telefono;
      existente.Email = vm.Email;

      _unitOfWork.UbicacionRepository.Upsert(existente);
      _unitOfWork.Save();
      return Ok(MapToDto(existente));
    }

    // DELETE api/ubicacion/Delete/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.UbicacionRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // -- Helpers ----------------------------------------

    private static UbicacionDto MapToDto(Ubicacion u) => new()
    {
      Id = u.Id,
      Nombre = u.Nombre,
      Direccion = u.Direccion,
      CodigoPostal = u.CodigoPostal,
      Ciudad = u.Ciudad,
      Provincia = u.Provincia,
      Telefono = u.Telefono,
      Email = u.Email,
      Disabled = u.Disabled,
    };

    private static Ubicacion MapToEntity(UbicacionDto vm) => new()
    {
      Id = vm.Id,
      Nombre = vm.Nombre,
      Direccion = vm.Direccion,
      CodigoPostal = vm.CodigoPostal,
      Ciudad = vm.Ciudad,
      Provincia = vm.Provincia,
      Telefono = vm.Telefono,
      Email = vm.Email,
    };
  }
}

