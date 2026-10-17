using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OdontoSoft.API.Filters;

/// <summary>
/// Filtro global que ejecuta los <see cref="IValidator{T}"/> registrados en
/// DI contra los argumentos de cada acción, antes de que el controller se
/// ejecute. Los errores se vuelcan al <see cref="Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary"/>;
/// después, el comportamiento automático de <c>[ApiController]</c> corta la
/// ejecución si el ModelState es inválido y el
/// <c>InvalidModelStateResponseFactory</c> configurado en <c>Program.cs</c>
/// los formatea con el shape <c>{ error, errors: { Field: [...] } }</c>.
///
/// Esto evita tener que llamar manualmente a cada <c>Validator.Validate(vm)</c>
/// desde los controllers, y centraliza el patrón de "validate-then-execute".
/// </summary>
public class FluentValidationFilter : IAsyncActionFilter
{
  private readonly IServiceProvider _serviceProvider;

  public FluentValidationFilter(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    foreach (var arg in context.ActionArguments.Values)
    {
      if (arg is null) continue;

      var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
      var validator = _serviceProvider.GetService(validatorType) as IValidator;
      if (validator is null) continue;

      var validationContext = new ValidationContext<object>(arg);
      var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
      if (result.IsValid) continue;

      foreach (var error in result.Errors)
      {
        // PropertyName viene en notación PascalCase; el factory lo va
        // a usar tal cual, así que queda consistente con lo que el
        // front ya consume (ej. "Nombre", "Dni", "HoraInicio").
        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
    }

    await next();
  }
}
