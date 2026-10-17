using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Validators;

/// <summary>
/// Reglas de <see cref="OdontogramaPiezaDto"/>.
/// </summary>
public class OdontogramaPiezaDtoValidator : AbstractValidator<OdontogramaPiezaDto>
{
  public OdontogramaPiezaDtoValidator()
  {
    RuleFor(x => x.HistoriaClinicaId)
      .GreaterThan(0).WithMessage("La historia clínica es obligatoria.");

    RuleFor(x => x.NumeroPieza)
      .InclusiveBetween(11, 85)
        .WithMessage("El número de pieza debe estar entre 11 y 85 (notación FDI).");
  }
}
