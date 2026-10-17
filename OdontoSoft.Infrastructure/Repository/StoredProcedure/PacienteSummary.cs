namespace OdontoSoft.Infrastructure.Repository.StoredProcedure;

/// <summary>
/// Record interno de Infrastructure que modela la salida del stored procedure
/// <c>get_pacientes_for_turno</c>. Es un shape de "data" de la base, no un
/// DTO de la API — el controller lo mapea a <c>PacienteDto</c> con AutoMapper.
///
/// ¿Por qué existe este record? Para que Infrastructure (la capa de
/// persistencia) no conozca los DTOs de Contracts. El stored procedure
/// devuelve columnas planas, y el controller es el responsable de
/// proyectarlas al shape que el front espera.
/// </summary>
public record PacienteSummary(
  int Id,
  string Nombre,
  string Apellido,
  string Dni,
  int? Edad);
