using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Services
{
  public interface ILocalidadServices
  {
    /// <summary>
    /// Descarga el catálogo crudo de localidades de la API georef de
    /// datos.gob.ar, sin tocar la base de datos.
    /// </summary>
    Task<GeorefResponseDto> SincronizarLocalidades();

    /// <summary>
    /// Sincroniza las localidades de la Argentina en la base de datos.
    ///
    /// Es idempotente: sólo inserta las localidades que faltan, resuelte
    /// por nombre de provincia y deduplicadas por (nombre, provincia).
    /// Con <paramref name="soloSimular"/> = true no escribe nada y devuelve
    /// sólo los contadores de lo que se insertaría.
    /// </summary>
    Task<LocalidadSyncResult> SincronizarLocalidadesAsync(bool soloSimular = false);
  }
}

