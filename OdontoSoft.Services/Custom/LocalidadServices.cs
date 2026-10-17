using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using Odontosoft.Services;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Contracts.Dtos;
using System.Text.Json;
using Newtonsoft.Json;
using OdontoSoft.Domain.Models;

namespace OdontoSoft.Services
{
  public partial class LocalidadServices : BaseServices, ILocalidadServices
  {
    public async Task<GeorefResponseDto> SincronizarLocalidades()
    {
      string url = "https://infra.datos.gob.ar/georef/localidades.json";
      GeorefResponseDto georefResponseDto = new GeorefResponseDto();
      try
      {
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
          throw new Exception("Error en la API de localidades");

        string json = await response.Content.ReadAsStringAsync();

        // Deserializo en tu clase wrapper
        georefResponseDto = JsonConvert.DeserializeObject<GeorefResponseDto>(json);

        return georefResponseDto;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}

