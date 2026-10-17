using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Common;

namespace OdontoSoft.Application.Services;

/// <summary>
/// Lógica de negocio de Pacientes.
/// </summary>
public interface IPacienteServices
{
  /// <summary>
  /// Valida que no exista otro paciente con el mismo DNI. Si excludePacienteId
  /// se pasa, se ignora ese paciente (útil en Update).
  /// </summary>
  Task<ServiceResult> ValidarDniUnicoAsync(string dni, int? excludePacienteId = null);
}
