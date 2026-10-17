using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Services
{
  public interface ILocalidadServices
  {
    Task<GeorefResponseDto> SincronizarLocalidades();
  }
}

