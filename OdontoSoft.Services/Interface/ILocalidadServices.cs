using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Services
{
  public interface ILocalidadServices
  {
    Task<GeorefResponseDto> SincronizarLocalidades();
  }
}

