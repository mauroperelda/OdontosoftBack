using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Validators;

/// <summary>
/// Reglas de <see cref="UbicacionDto"/>. Reemplaza el método estático
/// <c>Validate(vm)</c> que vivía en <c>UbicacionController</c>.
/// </summary>
public class UbicacionDtoValidator : AbstractValidator<UbicacionDto>
{
  public UbicacionDtoValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre es obligatorio.");

    RuleFor(x => x.Direccion)
      .NotEmpty().WithMessage("La dirección es obligatoria.");

    RuleFor(x => x.Ciudad)
      .NotEmpty().WithMessage("La ciudad es obligatoria.");

    RuleFor(x => x.Email)
      .EmailAddress().WithMessage("El email no tiene un formato válido.")
      .When(x => !string.IsNullOrWhiteSpace(x.Email));
  }
}
