using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OdontoSoft.Infrastructure.Filters;
using OdontoSoft.Application.Validation;
using OdontoSoft.Contracts.Dtos;
using Xunit;

namespace OdontoSoft.UnitTests.Validators;

/// <summary>
/// Tests del <see cref="FluentValidationFilter"/> y de los validators
/// críticos de Fase 3.2. Verifican que el filter vuelca los errores al
/// ModelState con el property name en PascalCase (que es como el front
/// los espera).
/// </summary>
public class FluentValidationFilterTests
{
  [Fact]
  public async Task ValidPaciente_DoesNotAddModelStateErrors_AndCallsNext()
  {
    var ctx = BuildActionContext(new PacienteDto
    {
      Nombre = "Lucía",
      Apellido = "Fernández",
      Dni = "11111111",
      FechaNacimiento = new DateTime(1990, 1, 1),
    });

    var nextCalled = false;
    var filter = new FluentValidationFilter(BuildServices());

    await filter.OnActionExecutionAsync(ctx, () =>
    {
      nextCalled = true;
      return Task.FromResult<ActionExecutedContext>(null!);
    });

    ctx.ModelState.IsValid.Should().BeTrue();
    nextCalled.Should().BeTrue("el filter no corta, deja que [ApiController] o el controller decidan.");
  }

  [Fact]
  public async Task InvalidPaciente_AddsFieldErrors_ToModelState()
  {
    var ctx = BuildActionContext(new PacienteDto
    {
      Nombre = "",
      Apellido = "  ",
      Dni = "abc",
      FechaNacimiento = null,
    });

    var filter = new FluentValidationFilter(BuildServices());

    await filter.OnActionExecutionAsync(ctx, () => Task.FromResult<ActionExecutedContext>(null!));

    ctx.ModelState.IsValid.Should().BeFalse();
    // Verificamos que las claves están en PascalCase, como el front las espera.
    ctx.ModelState.Keys.Should().Contain("Nombre");
    ctx.ModelState.Keys.Should().Contain("Apellido");
    ctx.ModelState.Keys.Should().Contain("Dni");
    ctx.ModelState.Keys.Should().Contain("FechaNacimiento");
  }

  [Fact]
  public async Task DniWithBadFormat_FailsValidation()
  {
    var validator = new PacienteDtoValidator();
    var result = await validator.ValidateAsync(new PacienteDto
    {
      Nombre = "Lucía",
      Apellido = "Fernández",
      Dni = "12",
      FechaNacimiento = new DateTime(1990, 1, 1),
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "Dni");
  }

  [Fact]
  public async Task FutureBirthDate_FailsValidation()
  {
    var validator = new PacienteDtoValidator();
    var result = await validator.ValidateAsync(new PacienteDto
    {
      Nombre = "Lucía",
      Apellido = "Fernández",
      Dni = "11111111",
      FechaNacimiento = DateTime.Today.AddDays(1),
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "FechaNacimiento");
  }

  [Fact]
  public async Task TurnoWithoutPaciente_FailsValidation()
  {
    var validator = new TurnoDtoValidator();
    var result = await validator.ValidateAsync(new TurnoDto
    {
      PacienteId = 0,
      ProfesionalId = 5,
      Fecha = DateTime.Today,
      HoraInicio = new DateTime(2026, 1, 1, 10, 0, 0),
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "PacienteId");
  }

  [Fact]
  public async Task TurnoFinBeforeInicio_FailsValidation()
  {
    var validator = new TurnoDtoValidator();
    var result = await validator.ValidateAsync(new TurnoDto
    {
      PacienteId = 1,
      ProfesionalId = 5,
      Fecha = DateTime.Today,
      HoraInicio = new DateTime(2026, 1, 1, 11, 0, 0),
      HoraFin = new DateTime(2026, 1, 1, 10, 0, 0),
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "HoraFin");
  }

  [Fact]
  public async Task PagoWithZeroMonto_FailsValidation()
  {
    var validator = new PagoDtoValidator();
    var result = await validator.ValidateAsync(new PagoDto
    {
      PresupuestoId = 1,
      Monto = 0,
      MetodoPago = "Efectivo",
      Fecha = DateTime.Today,
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "Monto");
  }

  [Fact]
  public async Task PiezaOutsideFdiRange_FailsValidation()
  {
    var validator = new OdontogramaPiezaDtoValidator();
    var result = await validator.ValidateAsync(new OdontogramaPiezaDto
    {
      HistoriaClinicaId = 1,
      NumeroPieza = 5, // fuera de rango FDI (11-85)
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "NumeroPieza");
  }

  [Fact]
  public async Task PiezaInFdiRange_PassesValidation()
  {
    var validator = new OdontogramaPiezaDtoValidator();
    var result = await validator.ValidateAsync(new OdontogramaPiezaDto
    {
      HistoriaClinicaId = 1,
      NumeroPieza = 16, // 1.6 FDI
    });
    result.IsValid.Should().BeTrue();
  }

  [Fact]
  public async Task UbicacionWithoutDireccion_FailsValidation()
  {
    var validator = new UbicacionDtoValidator();
    var result = await validator.ValidateAsync(new UbicacionDto
    {
      Nombre = "Consultorio Centro",
      Direccion = "",
      Ciudad = "CABA",
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "Direccion");
  }

  [Fact]
  public async Task ProfesionalWithoutDniInCreate_FailsValidation()
  {
    var validator = new ProfesionalDtoValidator();
    // Id == 0 → simula Create. Sin DNI debe fallar.
    var result = await validator.ValidateAsync(new ProfesionalDto
    {
      Id = 0,
      Nombre = "Ana",
      Apellido = "Gómez",
      Dni = "",
    });
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "Dni");
  }

  [Fact]
  public async Task ProfesionalWithoutDniInUpdate_PassesValidation()
  {
    var validator = new ProfesionalDtoValidator();
    // Id > 0 → simula Update. DNI opcional, no debe fallar.
    var result = await validator.ValidateAsync(new ProfesionalDto
    {
      Id = 5,
      Nombre = "Ana",
      Apellido = "Gómez",
      Dni = "",
    });
    result.IsValid.Should().BeTrue();
  }

  [Fact]
  public async Task FilterCallsNext_WhenModelStateIsValid()
  {
    var ctx = BuildActionContext(new PacienteDto
    {
      Nombre = "Lucía",
      Apellido = "Fernández",
      Dni = "11111111",
      FechaNacimiento = new DateTime(1990, 1, 1),
    });

    var nextCalled = false;
    var filter = new FluentValidationFilter(BuildServices());
    await filter.OnActionExecutionAsync(ctx, () =>
    {
      nextCalled = true;
      return Task.FromResult<ActionExecutedContext>(null!);
    });

    nextCalled.Should().BeTrue();
  }

  // ── Helpers ────────────────────────────────────────

  private static ActionExecutingContext BuildActionContext(object argument)
  {
    var httpContext = new DefaultHttpContext();
    var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    return new ActionExecutingContext(
      actionContext,
      new List<IFilterMetadata>(),
      new Dictionary<string, object?> { ["vm"] = argument },
      controller: new object());
  }

  private static IServiceProvider BuildServices()
  {
    var services = new ServiceCollection();
    // El filter hace GetService<IValidator<T>>(). El test necesita que
    // encuentre validators reales — los del API no son visibles desde
    // UnitTests sin una reference, así que registramos uno acá mismo.
    services.AddScoped<IValidator<PacienteDto>, PacienteDtoValidator>();
    services.AddScoped<IValidator<TurnoDto>, TurnoDtoValidator>();
    services.AddScoped<IValidator<PresupuestoDto>, PresupuestoDtoValidator>();
    services.AddScoped<IValidator<ProfesionalDto>, ProfesionalDtoValidator>();
    services.AddScoped<IValidator<HistoriaClinicaDto>, HistoriaClinicaDtoValidator>();
    services.AddScoped<IValidator<UbicacionDto>, UbicacionDtoValidator>();
    services.AddScoped<IValidator<PagoDto>, PagoDtoValidator>();
    services.AddScoped<IValidator<PresupuestoDetalleDto>, PresupuestoDetalleDtoValidator>();
    services.AddScoped<IValidator<OdontogramaPiezaDto>, OdontogramaPiezaDtoValidator>();
    services.AddScoped<IValidator<FichaTratamientoDto>, FichaTratamientoDtoValidator>();
    return services.BuildServiceProvider();
  }
}
