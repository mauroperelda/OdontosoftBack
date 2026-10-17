using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Validation;

/// <summary>
/// Reglas de <see cref="TurnoDto"/>. Reemplaza el bloque manual
/// que vivía en <c>TurnoController.Create</c>/<c>Update</c>.
/// La regla de "profesional ya tiene turno en ese horario" se sigue
/// resolviendo en el controller porque requiere una query a la DB
/// (no es validación de shape).
/// </summary>
public class TurnoDtoValidator : AbstractValidator<TurnoDto>
{
  public TurnoDtoValidator()
  {
    RuleFor(x => x.PacienteId)
      .GreaterThan(0).WithMessage("El paciente es obligatorio.");

    RuleFor(x => x.ProfesionalId)
      .GreaterThan(0).WithMessage("El profesional es obligatorio.");

    RuleFor(x => x.Fecha)
      .NotNull().WithMessage("La fecha es obligatoria.");

    RuleFor(x => x.HoraInicio)
      .NotNull().WithMessage("La hora de inicio es obligatoria.");

    // Si llega HoraFin, debe ser posterior a HoraInicio.
    RuleFor(x => x.HoraFin)
      .GreaterThan(x => x.HoraInicio!.Value)
        .When(x => x.HoraFin.HasValue && x.HoraInicio.HasValue)
        .WithMessage("La hora de fin debe ser posterior a la hora de inicio.");
  }
}
