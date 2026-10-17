using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Validation;

/// <summary>
/// Reglas de <see cref="PagoDto"/>. La validación cruzada de
/// "monto no supera saldo pendiente" se sigue haciendo en el controller
/// porque requiere leer el estado del presupuesto en la DB.
/// </summary>
public class PagoDtoValidator : AbstractValidator<PagoDto>
{
  public PagoDtoValidator()
  {
    RuleFor(x => x.PresupuestoId)
      .GreaterThan(0).WithMessage("El presupuesto es obligatorio.");

    RuleFor(x => x.Monto)
      .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");

    RuleFor(x => x.MetodoPago)
      .NotEmpty().WithMessage("El método de pago es obligatorio.");

    RuleFor(x => x.Fecha)
      .NotEqual(default(DateTime)).WithMessage("La fecha es obligatoria.");
  }
}
