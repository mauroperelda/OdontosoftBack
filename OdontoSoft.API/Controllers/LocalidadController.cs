using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mauro.RepositoryPattern.Infrastructure;
using OdontoSoft.API.Helpers;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Services;
using OdontoSoft.Contracts.Dtos;
using System.Data;

namespace OdontoSoft.API.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class LocalidadController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalidadServices _localidadServices;
    private readonly IConfiguration _configuration;

    public LocalidadController(IUnitOfWork unitOfWork, ILocalidadServices localidadServices, IConfiguration configuration)
    {
      _unitOfWork = unitOfWork;
      _localidadServices = localidadServices;
      _configuration = configuration;
    }

    // GET api/localidad
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Localidad>> GetAll()
    {
      var localidades = _unitOfWork.LocalidadRepository.GetAllEnabled().ToList();
      return Ok(localidades);
    }

    // GET api/localidad/5
    [HttpGet("GetById/{id}")]
    public ActionResult<Localidad> GetById(int id)
    {
      var localidad = _unitOfWork.LocalidadRepository.GetById(id);
      if (localidad == null) return ApiErrorResponse.NotFound("Localidad no encontrada.");
      return Ok(localidad);
    }

    // GET api/localidad/provincia/5
    [HttpGet("GetByProvincia/{provinciaId}")]
    public ActionResult<IEnumerable<Localidad>> GetByProvincia(int provinciaId)
    {
      var localidades = _unitOfWork.LocalidadRepository.GetByProvinciaId(provinciaId).ToList();
      return Ok(localidades);
    }

    // POST api/localidad
    [HttpPost("Create")]
    public ActionResult<Localidad> Create([FromBody] Localidad localidad)
    {
      _unitOfWork.LocalidadRepository.Add(localidad);
      _unitOfWork.Save();
      return CreatedAtAction(nameof(GetById), new { id = localidad.Id }, localidad);
    }

    // PUT api/localidad/5
    [HttpPut("Update/{id}")]
    public ActionResult<Localidad> Update(int id, [FromBody] Localidad localidad)
    {
      var existente = _unitOfWork.LocalidadRepository.GetById(id);
      if (existente == null) return ApiErrorResponse.NotFound("Localidad no encontrada.");
      localidad.Id = id;
      _unitOfWork.LocalidadRepository.Upsert(localidad);
      _unitOfWork.Save();
      return Ok(localidad);
    }

    // DELETE api/localidad/5
    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      _unitOfWork.LocalidadRepository.Delete(id);
      _unitOfWork.Save();
      return NoContent();
    }

    // GET api/localidad/sincronizar
    [HttpGet("SincronizarLocalidades")]
    public async Task<ActionResult<object>> SincronizarLocalidades()
    {
      var georefResponse = await _localidadServices.SincronizarLocalidades();
      BulkInsertLocalidades(georefResponse);
      return Ok(new { mensaje = "Localidades sincronizadas correctamente." });
    }

    // ── HELPERS ───────────────────────────────────────────────────────────────

    private void BulkInsertLocalidades(GeorefResponseDto georefResponseDto)
    {
      List<Provincia> provincias = _unitOfWork.ProvinciaRepository.GetAllEnabled().ToList();
      var provinciasDict = provincias.ToDictionary(
          p => NormalizeText(p.Descripcion),
          p => p.Id
      );

      List<Localidad> localidadesExistentes = _unitOfWork.LocalidadRepository.GetAllEnabled().ToList();
      var localidadesExistentesSet = new HashSet<string>(
          localidadesExistentes.Select(l =>
              NormalizeText(l.Descripcion) + "|" + l.ProvinciaId
          )
      );

      List<Localidad> localidadesAInsertar = new List<Localidad>();

      foreach (var loc in georefResponseDto.localidades)
      {
        string provinciaNombreNormalizado = NormalizeText(loc.provincia.nombre);
        if (provinciasDict.TryGetValue(provinciaNombreNormalizado, out int provinciaId))
        {
          string key = NormalizeText(loc.nombre) + "|" + provinciaId;
          if (!localidadesExistentesSet.Contains(key))
          {
            localidadesAInsertar.Add(new Localidad
            {
              Descripcion = loc.nombre,
              ProvinciaId = provinciaId,
              Disabled = false,
              ObjectState = ObjectState.Added
            });
            localidadesExistentesSet.Add(key);
          }
        }
      }

      if (localidadesAInsertar.Any())
      {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        using var bulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction);
        bulkCopy.DestinationTableName = "dbo.Localidad";
        bulkCopy.ColumnMappings.Add("Descripcion", "Descripcion");
        bulkCopy.ColumnMappings.Add("ProvinciaId", "ProvinciaId");
        bulkCopy.ColumnMappings.Add("Disabled", "Disabled");
        var dataTable = ToDataTable(localidadesAInsertar);
        bulkCopy.WriteToServer(dataTable);
      }
    }

    private DataTable ToDataTable(List<Localidad> localidades)
    {
      var table = new DataTable();
      table.Columns.Add("Descripcion", typeof(string));
      table.Columns.Add("ProvinciaId", typeof(int));
      table.Columns.Add("Disabled", typeof(bool));
      foreach (var loc in localidades)
        table.Rows.Add(loc.Descripcion, loc.ProvinciaId, loc.Disabled);
      return table;
    }

    private static string NormalizeText(string text)
    {
      if (string.IsNullOrEmpty(text)) return text;
      return text.Normalize(System.Text.NormalizationForm.FormD)
          .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
          .Aggregate(string.Empty, (s, c) => s + c)
          .ToUpperInvariant();
    }
  }
}
