using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;

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
        public IActionResult GetAll()
        {
            try
            {
                var items = _unitOfWork.UbicacionRepository.GetAllEnabled().ToList();
                var vms = items.Select(MapToViewModel).ToList();
                return Ok(vms);
            }
            catch (Exception ex)
            {
                return ApiErrorResponse.FromException(ex);
            }
        }

        // GET api/ubicacion/GetById/5
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var u = _unitOfWork.UbicacionRepository.GetById(id);
                if (u == null) return ApiErrorResponse.NotFound("Ubicación no encontrada.");
                return Ok(MapToViewModel(u));
            }
            catch (Exception ex)
            {
                return ApiErrorResponse.FromException(ex);
            }
        }

        // POST api/ubicacion/Create
        [HttpPost("Create")]
        public IActionResult Create([FromBody] UbicacionViewModel vm)
        {
            try
            {
                var fieldErrors = Validate(vm);
                if (fieldErrors.Count > 0)
                {
                    return ApiErrorResponse.BadRequest(
                        "Revisá los campos marcados.",
                        fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
                }

                var entity = MapToEntity(vm);
                _unitOfWork.UbicacionRepository.Add(entity);
                _unitOfWork.Save();
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapToViewModel(entity));
            }
            catch (Exception ex)
            {
                return ApiErrorResponse.FromException(ex, "Ubicacion");
            }
        }

        // PUT api/ubicacion/Update/5
        [HttpPut("Update/{id}")]
        public IActionResult Update(int id, [FromBody] UbicacionViewModel vm)
        {
            try
            {
                var fieldErrors = Validate(vm);
                if (fieldErrors.Count > 0)
                {
                    return ApiErrorResponse.BadRequest(
                        "Revisá los campos marcados.",
                        fieldErrors.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
                }

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
                return Ok(MapToViewModel(existente));
            }
            catch (Exception ex)
            {
                return ApiErrorResponse.FromException(ex, "Ubicacion");
            }
        }

        // DELETE api/ubicacion/Delete/5
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _unitOfWork.UbicacionRepository.Delete(id);
                _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return ApiErrorResponse.FromException(ex, "Ubicacion");
            }
        }

        // ── Helpers ────────────────────────────────────────

        private static Dictionary<string, string> Validate(UbicacionViewModel vm)
        {
            var fe = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(vm.Nombre)) fe["Nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(vm.Direccion)) fe["Direccion"] = "La dirección es obligatoria.";
            if (string.IsNullOrWhiteSpace(vm.Ciudad)) fe["Ciudad"] = "La ciudad es obligatoria.";
            return fe;
        }

        private static UbicacionViewModel MapToViewModel(Ubicacion u) => new()
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

        private static Ubicacion MapToEntity(UbicacionViewModel vm) => new()
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
