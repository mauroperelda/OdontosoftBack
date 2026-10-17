using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Common;

using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services;

/// <summary>
/// Lógica de negocio de Pacientes. Hoy la validación de DNI único
/// termina como un DbUpdateException con SqlState 23505 que captura
/// ApiErrorResponse.FromException; este service permite chequearlo
/// antes del INSERT para devolver un 400 con field-keyed error.
/// </summary>
public partial class PacienteServices
{
  public Task<ServiceResult> ValidarDniUnicoAsync(string dni, int? excludePacienteId = null)
  {
    if (string.IsNullOrWhiteSpace(dni))
      return Task.FromResult(ServiceResult.Fail("El DNI es obligatorio."));

    var query = _unitOfWork.PacienteRepository.GetAllEnabled()
        .Where(p => p.Dni == dni);

    if (excludePacienteId.HasValue)
      query = query.Where(p => p.Id != excludePacienteId.Value);

    var exists = query.Any();
    if (exists)
    {
      return Task.FromResult(ServiceResult.Fail(
          $"Ya existe un paciente con DNI {dni}.",
          new Dictionary<string, string> { ["Dni"] = $"Ya existe un paciente con DNI {dni}." }));
    }
    return Task.FromResult(ServiceResult.Ok());
  }
}
