using OdontoSoft.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Helpers;

namespace OdontoSoft.Infrastructure.Repository.StoredProcedure;

/// <summary>
/// Implementación de las queries a stored procedures que no encajan en el
/// modelo entidad/repositorio tradicional. Devuelven <see cref="PacienteSummary"/>
/// (record interno de Infrastructure) en lugar de DTOs de Contracts, para
/// que esta capa no conozca la shape de la API. El controller es responsable
/// de mapear PacienteSummary → PacienteDto con AutoMapper.
///
/// La PL/pgSQL function <c>get_pacientes_for_turno</c> existe en la base
/// (ver migration CreateGetPacientesForTurnoFunction) por si en el futuro
/// queremos exponerla a otros clientes. Acá usamos LINQ directo porque
/// EF Core 9 no soporta <c>Database.SqlQueryRaw&lt;T&gt;</c> para tipos
/// complejos (eso es EF Core 10).
/// </summary>
public class StoredProcedureRepository : IStoredProcedureQueries
{
  protected OdontosoftContext _context;
  protected readonly ILogger<StoredProcedureRepository> _logger;

  // ILogger<StoredProcedureRepository> (genérico) y no ILogger pelado: el
  // contenedor de DI de ASP.NET Core registra ILogger<T> e ILoggerFactory,
  // pero NUNCA el ILogger no-genérico. Pedir el no-genérico acá hacía que
  // ValidateOnBuild (activo en Development) tirara al construir el provider
  // y la API no levantaba el puerto.
  public StoredProcedureRepository(OdontosoftContext context, ILogger<StoredProcedureRepository> logger)
  {
    _context = context;
    _logger = logger;
  }

  public IList<PacienteSummary> GetPacienteForTurno(string searchText)
  {
    var query = _context.Paciente.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(searchText))
    {
      var like = searchText.Trim().ToLower();
      query = query.Where(p =>
          p.Nombre.ToLower().Contains(like)
          || p.Apellido.ToLower().Contains(like)
          || p.Dni.Contains(searchText.Trim()));
    }

    return query
        .Where(p => !p.Disabled)
        .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
        .Take(50)
        .Select(p => new PacienteSummary(
            p.Id,
            p.Nombre,
            p.Apellido,
            p.Dni,
            null))  // Edad se calcularía con FechaNacimiento si la tuviéramos
        .ToList();
  }
}
