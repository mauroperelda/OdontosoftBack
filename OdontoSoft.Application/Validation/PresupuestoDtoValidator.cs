using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Validation;

/// <summary>
/// Reglas de <see cref="PresupuestoDto"/>. Reemplaza los bloques
/// manuales en <c>PresupuestoController.Create</c>/<c>Update</c>.
/// </summary>
public class PresupuestoDtoValidator : AbstractValidator<PresupuestoDto>
{
  public PresupuestoDtoValidator()
  {
    RuleFor(x => x.PacienteId)
      .GreaterThan(0).WithMessage("El paciente es obligatorio.");

    // ProfesionalId es opcional en Create (la columna en DB es NULL).
    // En Update se exige > 0 si el front lo está mandando — se valida
    // en el controller porque depende del contexto (no del shape).
  }
}
