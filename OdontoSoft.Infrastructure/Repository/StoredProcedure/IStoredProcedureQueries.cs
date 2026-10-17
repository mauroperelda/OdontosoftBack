namespace OdontoSoft.Infrastructure.Repository.StoredProcedure;

/// <summary>
/// Queries a stored procedures que devuelven shapes específicos de la
/// base (no entidades de dominio). Vive en Infrastructure (no en
/// Application) porque sus tipos de retorno son específicos de la
/// implementación y no deben filtrarse al dominio.
///
/// El controller mapea cada resultado a su DTO con AutoMapper.
/// </summary>
public interface IStoredProcedureQueries
{
  /// <summary>
  /// Búsqueda de pacientes para el autocomplete del modal "Nuevo turno".
  /// Llama al stored procedure <c>get_pacientes_for_turno</c> que matchea
  /// por DNI o por nombre/apellido con ILIKE.
  /// </summary>
  IList<PacienteSummary> GetPacienteForTurno(string searchText);
}
