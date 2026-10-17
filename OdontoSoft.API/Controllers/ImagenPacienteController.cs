using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class ImagenPacienteController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ImagenPacienteController> _logger;

    // Límite duro: lo suaviza FormOptions pero igual validamos acá por las dudas.
    private const long MaxFileSize = 10 * 1024 * 1024;

    // MIME permitidos para evitar que suban PDF, .exe, etc.
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
      "image/jpeg",
      "image/jpg",
      "image/png",
      "image/webp",
    };

    public ImagenPacienteController(
      IUnitOfWork unitOfWork,
      IWebHostEnvironment env,
      ILogger<ImagenPacienteController> logger)
    {
      _unitOfWork = unitOfWork;
      _env = env;
      _logger = logger;
    }

    // GET api/imagenpaciente/historiaclinica/5
    [HttpGet("GetByHistoriaClinica/{historiaClinicaId}")]
    public ActionResult<IEnumerable<ImagenPacienteDto>> GetByHistoriaClinica(int historiaClinicaId)
    {
      var imagenes = _unitOfWork.ImagenPacienteRepository
        .GetByHistoriaClinicaId(historiaClinicaId);
      return Ok(imagenes.Select(i => new ImagenPacienteDto(i)).ToList());
    }

    // GET api/imagenpaciente/5
    [HttpGet("GetById/{id}")]
    public ActionResult<ImagenPacienteDto> GetById(int id)
    {
      var img = _unitOfWork.ImagenPacienteRepository.GetById(id);
      if (img == null) return ApiErrorResponse.NotFound("Imagen no encontrada.");
      return Ok(new ImagenPacienteDto(img));
    }

    // POST api/imagenpaciente/Upload/5
    // Acepta un solo archivo en `archivo` (multipart/form-data).
    [HttpPost("Upload/{historiaClinicaId}")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<ImagenPacienteDto>> Upload(int historiaClinicaId, IFormFile archivo, [FromForm] string? descripcion)
    {
      if (archivo == null || archivo.Length == 0)
        return ApiErrorResponse.BadRequest("No se recibió ningún archivo.");

      if (archivo.Length > MaxFileSize)
        return ApiErrorResponse.BadRequest($"El archivo supera el máximo permitido de {MaxFileSize / 1024 / 1024} MB.");

      // Algunos navegadores mandan application/octet-stream en vez del MIME real,
      // así que también validamos por extensión.
      var mime = (archivo.ContentType ?? "").ToLowerInvariant();
      var ext = Path.GetExtension(archivo.FileName)?.ToLowerInvariant() ?? "";
      var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
      if (!AllowedMimeTypes.Contains(mime) && !allowedExt.Contains(ext))
        return ApiErrorResponse.BadRequest("Tipo de archivo no permitido. Solo se aceptan imágenes JPG, PNG o WEBP.");

      // Construir ruta física: wwwroot/uploads/pacientes/{hcId}/{guid}.{ext}
      // TODO(roadmap): migrar a IFileStorage abstraction (S3 / R2 / Supabase Storage).
      //   Hoy escribe a wwwroot que es EFÍMERO en Render (los archivos se
      //   pierden en cada redeploy porque el FS de Render no persiste).
      //   Pasos sugeridos cuando se haga:
      //     1. Crear OdontoSoft.Data/Storage/IFileStorage.cs con
      //        Task<string> SaveAsync(Stream content, string path, string contentType, CancellationToken ct).
      //     2. Implementar LocalFileStorage (hoy) y S3FileStorage (después).
      //     3. Registrar IFileStorage en DI según la config (Storage:Provider = "local"|"s3").
      //     4. Cambiar este controller para que inyecte IFileStorage en vez
      //        de tocar _env.WebRootPath.
      //     5. Agregar migration que mueva la columna RutaArchivo a una URL
      //        completa (o mantenerla relativa y resolver en runtime).
      var folder = Path.Combine(_env.WebRootPath, "uploads", "pacientes", historiaClinicaId.ToString());
      Directory.CreateDirectory(folder);

      var fileName = $"{Guid.NewGuid():N}{ext}";
      var physicalPath = Path.Combine(folder, fileName);

      await using (var stream = System.IO.File.Create(physicalPath))
      {
        await archivo.CopyToAsync(stream);
      }

      var rutaRelativa = Path.Combine("uploads", "pacientes", historiaClinicaId.ToString(), fileName)
        .Replace('\\', '/');

      var entity = new ImagenPaciente
      {
        HistoriaClinicaId = historiaClinicaId,
        NombreArchivo = archivo.FileName,
        RutaArchivo = rutaRelativa,
        MimeType = string.IsNullOrEmpty(mime) ? "application/octet-stream" : mime,
        TamanoBytes = archivo.Length,
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion,
        FechaSubida = DateTime.UtcNow,
      };
      _unitOfWork.ImagenPacienteRepository.Add(entity);
      _unitOfWork.Save();

      return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new ImagenPacienteDto(entity));
    }

    // PUT api/imagenpaciente/5
    // Solo edita la descripción (no permitimos re-subir el archivo binario).
    [HttpPut("Update/{id}")]
    public ActionResult<ImagenPacienteDto> Update(int id, [FromBody] ImagenPacienteDto vm)
    {
      var entity = _unitOfWork.ImagenPacienteRepository.GetById(id);
      if (entity == null) return ApiErrorResponse.NotFound("Imagen no encontrada.");

      entity.Descripcion = vm.Descripcion;
      _unitOfWork.ImagenPacienteRepository.Upsert(entity);
      _unitOfWork.Save();

      return Ok(new ImagenPacienteDto(entity));
    }

    // DELETE api/imagenpaciente/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      var entity = _unitOfWork.ImagenPacienteRepository.GetById(id);
      if (entity == null) return ApiErrorResponse.NotFound("Imagen no encontrada.");

      // Borra el archivo físico si existe. Si no existe, sigue con la baja lógica.
      string? physical = null;
      try
      {
        physical = Path.Combine(_env.WebRootPath, entity.RutaArchivo.Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(physical))
          System.IO.File.Delete(physical);
      }
      catch (Exception fileEx)
      {
        // No abortamos la operación si falla el delete del archivo: el registro igual se va.
        _logger.LogError(fileEx, "No se pudo borrar el archivo físico {Path}", physical);
      }

      _unitOfWork.ImagenPacienteRepository.Delete(entity.Id);
      _unitOfWork.Save();
      return NoContent();
    }

    // GET api/imagenpaciente/Download/5
    // Devuelve el binario. No usamos app.UseStaticFiles() (no está configurado),
    // por eso el controller sirve el archivo directamente.
    [HttpGet("Download/{id}")]
    public IActionResult Download(int id)
    {
      var entity = _unitOfWork.ImagenPacienteRepository.GetById(id);
      if (entity == null) return ApiErrorResponse.NotFound("Imagen no encontrada.");

      var physical = Path.Combine(_env.WebRootPath, entity.RutaArchivo.Replace('/', Path.DirectorySeparatorChar));
      if (!System.IO.File.Exists(physical))
        return ApiErrorResponse.NotFound("Archivo físico no encontrado en disco.");

      var stream = new FileStream(physical, FileMode.Open, FileAccess.Read);
      return File(stream, string.IsNullOrWhiteSpace(entity.MimeType) ? "application/octet-stream" : entity.MimeType, entity.NombreArchivo);
    }
  }
}
