using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Validators;

/// <summary>
/// Reglas de <see cref="HistoriaClinicaDto"/>.
/// </summary>
public class HistoriaClinicaDtoValidator : AbstractValidator<HistoriaClinicaDto>
{
  public HistoriaClinicaDtoValidator()
  {
    RuleFor(x => x.PacienteId)
      .GreaterThan(0).WithMessage("El paciente es obligatorio.");
  }
}
