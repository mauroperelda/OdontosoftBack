using FluentValidation;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Application.Validation;

/// <summary>
/// Reglas de <see cref="PresupuestoDetalleDto"/>.
/// </summary>
public class PresupuestoDetalleDtoValidator : AbstractValidator<PresupuestoDetalleDto>
{
  public PresupuestoDetalleDtoValidator()
  {
    RuleFor(x => x.PresupuestoId)
      .GreaterThan(0).WithMessage("El presupuesto es obligatorio.");

    RuleFor(x => x.TratamientoId)
      .GreaterThan(0).WithMessage("El tratamiento es obligatorio.");

    RuleFor(x => x.Cantidad)
      .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

    RuleFor(x => x.PrecioUnitario)
      .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");
  }
}
