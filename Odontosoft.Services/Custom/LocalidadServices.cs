using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using Odontosoft.Services;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Entities.ViewModels;
using System.Text.Json;
using Newtonsoft.Json;
using OdontoSoft.Entities.Models;

namespace OdontoSoft.Services
{
    public partial class LocalidadServices : BaseServices, ILocalidadServices
    {
        public async Task<GeorefResponseViewModel> SincronizarLocalidades()
        {
            string url = "https://infra.datos.gob.ar/georef/localidades.json";
            GeorefResponseViewModel georefResponseViewModel = new GeorefResponseViewModel();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error en la API de localidades");

                string json = await response.Content.ReadAsStringAsync();

                // Deserializo en tu clase wrapper
                georefResponseViewModel = JsonConvert.DeserializeObject<GeorefResponseViewModel>(json);

                return georefResponseViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

