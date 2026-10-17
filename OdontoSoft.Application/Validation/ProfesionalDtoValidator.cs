using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Validation;

/// <summary>
/// Reglas de <see cref="ProfesionalDto"/>. Reemplaza los bloques
/// manuales en <c>ProfesionalController.Create</c>/<c>Update</c>.
/// </summary>
public class ProfesionalDtoValidator : AbstractValidator<ProfesionalDto>
{
  public ProfesionalDtoValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre es obligatorio.");

    RuleFor(x => x.Apellido)
      .NotEmpty().WithMessage("El apellido es obligatorio.");

    // DNI solo es obligatorio en Create. En Update permitimos que no
    // venga para no romper el flujo de "solo cambio el teléfono".
    // Separamos en dos RuleFor porque los .When() encadenados se acumulan
    // sobre la regla entera, no sobre cada cláusula.
    RuleFor(x => x.Dni)
      .NotEmpty().WithMessage("El DNI es obligatorio.")
      .When(x => x.Id == 0);

    RuleFor(x => x.Dni)
      .Matches(@"^\d{7,8}$").WithMessage("El DNI debe tener 7 u 8 dígitos.")
      .When(x => !string.IsNullOrWhiteSpace(x.Dni));

    RuleFor(x => x.Email)
      .EmailAddress().WithMessage("El email no tiene un formato válido.")
      .When(x => !string.IsNullOrWhiteSpace(x.Email));
  }
}
