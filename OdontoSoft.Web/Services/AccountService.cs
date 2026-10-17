//using DocumentFormat.OpenXml.ExtendedProperties;
//using DocumentFormat.OpenXml.Spreadsheet;
//using ExtraCore.Web.Exceptions;
//using ExtraCore.Web.Helpers;
//using ExtraCore.Web.Helpers.Authentication;
//using ExtraCore.Web.Helpers.Authentication.DTO;
//using ExtraCore.Web.IServices;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using OdontoSoft.Web.Exceptions;
using OdontoSoft.Web.Helpers.Authentication.DTO;
using OdontoSoft.Web.IServices;
using System.Net.Http.Headers;
using System.Text;

namespace OdontoSoft.Web.Services
{
  public class AccountService : IAccountService
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string pathRoot;
    //private ManagerConfiguration _managerConfiguration;
    public AccountService(HttpClient httpClient, IConfiguration configuration)
    {

      _httpClient = httpClient;
      pathRoot = "Account";
      _configuration = configuration;
      _httpClient.BaseAddress = new Uri(_configuration["loginUserAPI"]);
      //_httpClient.BaseAddress = new Uri(_managerConfiguration.Endpoint);
    }

    public async Task<UserDTO> LoginAsync(LoginDTO login)
    {

      //try
      //{ 
      //    // Datos del usuario para el inicio de sesión
      //    var loginDto = login;

      //    // Serializar el objeto LoginDTO a JSON
      //    string jsonLoginDto = Newtonsoft.Json.JsonConvert.SerializeObject(loginDto);

      //    // Crear una instancia de HttpClient



      //    // Configurar el encabezado de contenido para JSON
      //    _httpClient.DefaultRequestHeaders.Accept.Clear();
      //    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      //    // Enviar la solicitud POST con los datos de inicio de sesión
      //    string uri = $"{pathRoot}/Login";
      //    HttpResponseMessage response = await _httpClient.PostAsync(uri, new StringContent(jsonLoginDto, Encoding.UTF8, "application/json"));

      //    // Leer la respuesta como cadena JSON
      //    string jsonResponse = await response.Content.ReadAsStringAsync();

      //    // Verificar el código de estado de la respuesta
      //    if (response.IsSuccessStatusCode)
      //    {
      //        // Si la solicitud fue exitosa, deserializar la respuesta a un objeto UserDTO
      //        var userDto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(jsonResponse);

      //        return userDto;
      //    }
      //    else
      //    {
      //        throw new Exception(jsonResponse);

      //    }

      //}
      //catch (Exception ex)
      //{
      //    throw ex;

      //}

      try
      {
        //ServiceHandlerCore<LoginDTO> service = new ServiceHandlerCore<LoginDTO>(_httpClient);
        //string uri = $"{pathRoot}/Login";
        //UserDTO response = await service.PostAsyncJson<UserDTO>(uri, login);

        //return response;
        return null;
      }
      catch (Exception ex)
      {

        throw ex;
      }


    }



    public UserDTO Login(LoginDTO login)
    {

      try
      {
        // Datos del usuario para el inicio de sesión
        var loginDto = login;

        // Serializar el objeto LoginDTO a JSON
        string jsonLoginDto = Newtonsoft.Json.JsonConvert.SerializeObject(loginDto);

        // Crear una instancia de HttpClient



        // Configurar el encabezado de contenido para JSON
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Enviar la solicitud POST con los datos de inicio de sesión
        string uri = $"{pathRoot}/Login";
        HttpResponseMessage response = _httpClient.PostAsync(uri, new StringContent(jsonLoginDto, Encoding.UTF8, "application/json")).Result;

        // Leer la respuesta como cadena JSON
        string jsonResponse = response.Content.ReadAsStringAsync().Result;

        // Verificar el código de estado de la respuesta
        if (response.IsSuccessStatusCode)
        {
          // Si la solicitud fue exitosa, deserializar la respuesta a un objeto UserDTO
          var userDto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(jsonResponse);

          return userDto;
        }
        else
        {
          throw GetException(jsonResponse);
        }

      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    private Exception GetException(string apiResult)
    {
      apiResult = apiResult.Replace("\"", "");

      if (apiResult == "El usuario no existe") return new UsuarioContraseniaException(apiResult);
      if (apiResult == "Contraseña incorrecta") return new UsuarioContraseniaException(apiResult);
      if (apiResult == "El usuario está deshabilitado") return new UsuarioContraseniaException(apiResult);
      if (apiResult == "El usuario no está habilitado para sistemas de esta empresa") return new UsuarioContraseniaException(apiResult);
      if (apiResult == "El usuario no está habilitado para este sistema") return new UsuarioContraseniaException(apiResult);
      return new Exception(apiResult);
    }
  }
}
