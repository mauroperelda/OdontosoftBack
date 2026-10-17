using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Validation;

/// <summary>
/// Reglas de validación de <see cref="PacienteDto"/>.
/// Reemplaza el bloque <c>var fieldErrors = new Dictionary&lt;string, string&gt;();</c>
/// que estaba hardcoded en <c>PacienteController.Create</c>/<c>Update</c>.
/// </summary>
public class PacienteDtoValidator : AbstractValidator<PacienteDto>
{
  public PacienteDtoValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre es obligatorio.");

    RuleFor(x => x.Apellido)
      .NotEmpty().WithMessage("El apellido es obligatorio.");

    RuleFor(x => x.Dni)
      .NotEmpty().WithMessage("El DNI es obligatorio.")
      .Matches(@"^\d{7,8}$").WithMessage("El DNI debe tener 7 u 8 dígitos.");

    RuleFor(x => x.FechaNacimiento)
      .NotNull().WithMessage("La fecha de nacimiento es obligatoria.")
      .Must(f => f is null || f.Value.Date <= DateTime.Today)
        .WithMessage("La fecha de nacimiento no puede ser futura.");

    RuleFor(x => x.Email)
      .EmailAddress().WithMessage("El email no tiene un formato válido.")
      .When(x => !string.IsNullOrWhiteSpace(x.Email));
  }
}
