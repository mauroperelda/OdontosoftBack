using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;

using OdontoSoft.Contracts.Dtos;
using System.Text.Json;
using OdontoSoft.Domain.Models;

using OdontoSoft.Application.Services.Base;
using OdontoSoft.Application.Common;

namespace OdontoSoft.Application.Services
{
  public partial class LocalidadServices : BaseServices, ILocalidadServices
  {
    /// <summary>
    /// API oficial de datos.gob.ar con el catálogo de localidades de la
    /// Argentina (4.028 localidades, 24 provincias).
    /// </summary>
    private const string GeorefUrl = "https://infra.datos.gob.ar/georef/localidades.json";

    /// <summary>
    /// Opciones de deserialización. El JSON de georef usa nombres en
    /// minúsculas que coinciden con las propiedades del DTO; el
    /// case-insensitive deja margen por si el origen cambia el casing.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public async Task<GeorefResponseDto> SincronizarLocalidades()
    {
      GeorefResponseDto georefResponseDto = new GeorefResponseDto();
      try
      {
        HttpResponseMessage response = await _httpClient.GetAsync(GeorefUrl);

        if (!response.IsSuccessStatusCode)
          throw new Exception($"Error en la API de localidades: {(int)response.StatusCode} {response.StatusCode}");

        string json = await response.Content.ReadAsStringAsync();

        // Deserializo en tu clase wrapper.
        // Se usa System.Text.Json (y no Newtonsoft) a propósito: Newtonsoft
        // llegaba a este proyecto sólo como dependencia transitiva del .dll
        // legacy MedveCore.Common, y en los hosts donde no se copiaba al
        // output fallaba con FileNotFoundException al deserializar.
        georefResponseDto = JsonSerializer.Deserialize<GeorefResponseDto>(json, JsonOptions)
                            ?? new GeorefResponseDto();

        return georefResponseDto;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public async Task<LocalidadSyncResult> SincronizarLocalidadesAsync(bool soloSimular = false)
    {
      GeorefResponseDto georef = await SincronizarLocalidades();

      // Provincias contra las que se resuelve cada localidad de la fuente.
      List<Provincia> provincias = _unitOfWork.ProvinciaRepository.GetAllEnabled().ToList();

      // Claves de las localidades que ya están en la base. Se proyecta a un
      // tipo anónimo primero porque BuildKey es una función de C# y EF no la
      // puede traducir a SQL.
      List<string> clavesExistentes = _unitOfWork.LocalidadRepository.All()
          .Select(l => new { l.Descripcion, l.ProvinciaId })
          .ToList()
          .Select(l => LocalidadSyncMapper.BuildKey(l.Descripcion, l.ProvinciaId))
          .ToList();

      LocalidadSyncPlan plan = LocalidadSyncMapper.Map(georef.localidades, provincias, clavesExistentes);

      LocalidadSyncResult resultado = plan.Resultado;
      resultado.SoloSimular = soloSimular;

      if (!soloSimular && plan.Localidades.Count > 0)
      {
        resultado.Insertadas = _unitOfWork.LocalidadRepository.BulkInsert(plan.Localidades);
      }

      return resultado;
    }
  }
}
