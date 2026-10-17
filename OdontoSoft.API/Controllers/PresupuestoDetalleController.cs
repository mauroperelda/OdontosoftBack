using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class PresupuestoDetalleController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public PresupuestoDetalleController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    // GET api/presupuestodetalle
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<PresupuestoDetalle>> GetAll()
    {
      var detalles = _unitOfWork.PresupuestoDetalleRepository.GetAllEnabled().ToList();
      return Ok(detalles);
    }

    // GET api/presupuestodetalle/5
    [HttpGet("GetById/{id}")]
    public ActionResult<PresupuestoDetalle> GetById(int id)
    {
      var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
      if (detalle == null) return ApiErrorResponse.NotFound("Detalle no encontrado.");
      return Ok(detalle);
    }

    // POST api/presupuestodetalle
    [HttpPost("Create")]
    public ActionResult<PresupuestoDetalle> Create([FromBody] PresupuestoDetalleDto vm)
    {
      var detalle = new PresupuestoDetalle();
      vm.MapDtoToDetalle(detalle);
      _unitOfWork.PresupuestoDetalleRepository.Add(detalle);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, detalle);
    }

    // PUT api/presupuestodetalle/5
    [HttpPut("Update/{id}")]
    public ActionResult<PresupuestoDetalle> Update(int id, [FromBody] PresupuestoDetalleDto vm)
    {
      var detalle = _unitOfWork.PresupuestoDetalleRepository.GetById(id);
      if (detalle == null) return ApiErrorResponse.NotFound("Detalle no encontrado.");
      vm.Id = id;
      vm.MapDtoToDetalle(detalle);
      _unitOfWork.PresupuestoDetalleRepository.Upsert(detalle);
      _unitOfWork.Save();
      return Ok(detalle);
    }

    // DELETE api/presupuestodetalle/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.PresupuestoDetalleRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }
  }
}
