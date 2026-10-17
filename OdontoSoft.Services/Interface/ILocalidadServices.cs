using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Services
{
  public interface ILocalidadServices
  {
    Task<GeorefResponseDto> SincronizarLocalidades();
  }
}

