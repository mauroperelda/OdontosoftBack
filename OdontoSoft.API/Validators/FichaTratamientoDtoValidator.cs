using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Validators;

/// <summary>
/// Reglas de <see cref="FichaTratamientoDto"/>.
/// </summary>
public class FichaTratamientoDtoValidator : AbstractValidator<FichaTratamientoDto>
{
  public FichaTratamientoDtoValidator()
  {
    RuleFor(x => x.HistoriaClinicaId)
      .GreaterThan(0).WithMessage("La historia clínica es obligatoria.");

    RuleFor(x => x.TratamientoId)
      .GreaterThan(0).WithMessage("El tratamiento es obligatorio.");
  }
}
