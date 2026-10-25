using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using OdontoSoft.Infrastructure.Mapping;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using Xunit;

namespace OdontoSoft.UnitTests.Mapper;

/// <summary>
/// Tests del <see cref="ApplicationProfile"/>. Verifican que los mappings
/// bidireccionales VM ↔ Entity copian los campos correctos, y que las
/// custom rules (DomicilioNumero, Subtotal) se aplican.
/// </summary>
public class ApplicationProfileTests
{
  // AutoMapper 15+ requiere ILoggerFactory en el constructor de MapperConfiguration.
  // Pasamos NullLoggerFactory para no spammear en los logs de los tests.
  private static IMapper CreateMapper() =>
    new MapperConfiguration(
      cfg => cfg.AddProfile<ApplicationProfile>(),
      NullLoggerFactory.Instance)
    .CreateMapper();

  [Fact]
  public void Paciente_ViewModelToEntity_CopiesAllScalarFields()
  {
    var mapper = CreateMapper();
    var vm = new PacienteDto
    {
      Nombre = "Lucía",
      Apellido = "Fernández",
      Dni = "11111111",
      FechaNacimiento = new DateTime(1990, 1, 1),
      DomicilioCalle = "Av. Corrientes",
      DomicilioNumero = 1234,
      Telefono = "555-1234",
      Email = "lucia@test.com",
    };

    var entity = mapper.Map<Paciente>(vm);

    entity.Nombre.Should().Be("Lucía");
    entity.Apellido.Should().Be("Fernández");
    entity.Dni.Should().Be("11111111");
    entity.FechaNacimiento.Should().Be(new DateTime(1990, 1, 1));
    entity.DomicilioCalle.Should().Be("Av. Corrientes");
    entity.DomicilioNumero.Should().Be(1234, "el mapping custom ForMember debe copiar DomicilioNumero.");
    entity.Telefono.Should().Be("555-1234");
    entity.Email.Should().Be("lucia@test.com");
  }

  [Fact]
  public void Paciente_EntityToViewModel_CopiesAllScalarFields()
  {
    var mapper = CreateMapper();
    var entity = new Paciente
    {
      Nombre = "Lucía",
      Apellido = "Fernández",
      Dni = "11111111",
      DomicilioCalle = "Av. Corrientes",
      DomicilioNumero = 1234,
      Email = "lucia@test.com",
    };

    var vm = mapper.Map<PacienteDto>(entity);

    vm.Nombre.Should().Be("Lucía");
    vm.Apellido.Should().Be("Fernández");
    vm.Dni.Should().Be("11111111");
    vm.DomicilioCalle.Should().Be("Av. Corrientes");
    vm.DomicilioNumero.Should().Be(1234);
    vm.Email.Should().Be("lucia@test.com");
  }

  [Fact]
  public void Profesional_ViewModelToEntity_MapsDomicilioCalleToDomicilio()
  {
    var mapper = CreateMapper();
    var vm = new ProfesionalDto
    {
      Nombre = "Ana",
      Apellido = "Gómez",
      Dni = "22222222",
      DomicilioCalle = "Av. Rivadavia 5000",
      Cuit = "27-22222222-3",
    };

    var entity = mapper.Map<Profesional>(vm);

    entity.Nombre.Should().Be("Ana");
    entity.Domicilio.Should().Be("Av. Rivadavia 5000",
      "Profesional.Domicilio viene de la propiedad VM.DomicilioCalle.");
    entity.Cuit.Should().Be("27-22222222-3");
  }

  [Fact]
  public void PresupuestoDetalle_ViewModelToEntity_CalculatesSubtotal()
  {
    var mapper = CreateMapper();
    var vm = new PresupuestoDetalleDto
    {
      PresupuestoId = 1,
      TratamientoId = 5,
      Cantidad = 3,
      PrecioUnitario = 100.50m,
      // Subtotal en la VM se ignora al mapear (lo recalculamos).
      Subtotal = 999m,
    };

    var entity = mapper.Map<PresupuestoDetalle>(vm);

    entity.Subtotal.Should().Be(301.50m, "Subtotal = Cantidad * PrecioUnitario (regla de negocio).");
  }

  [Fact]
  public void Ubicacion_ViewModelToEntity_PreservesAllFields()
  {
    var mapper = CreateMapper();
    var vm = new UbicacionDto
    {
      Nombre = "Consultorio Centro",
      Direccion = "Av. 9 de Julio 1000",
      Ciudad = "CABA",
      CodigoPostal = "C1043",
      Email = "centro@odontosoft.com",
    };

    var entity = mapper.Map<Ubicacion>(vm);

    entity.Nombre.Should().Be("Consultorio Centro");
    entity.Direccion.Should().Be("Av. 9 de Julio 1000");
    entity.Ciudad.Should().Be("CABA");
    entity.CodigoPostal.Should().Be("C1043");
    entity.Email.Should().Be("centro@odontosoft.com");
  }

  [Fact]
  public void Mapper_ConfigurationIsValid_NoMissingMappings()
  {
    // Este test validaba una MapperConfiguration VACÍA (un no-op que nunca
    // podía fallar). Ahora verifica el perfil real: cada type map registrado
    // tiene que existir y ser utilizable.
    //
    // NO usamos AssertConfigurationIsValid() porque además de type maps
    // faltantes o incompatibles, exige que TODOS los members de destino
    // estén mapeados, y hay varios que legítimamente no lo están:
    //  - campos de auditoría (ObjectState, CreatedUserId, CreatedDate...):
    //    los setea ApplyAudit() del DbContext en SaveChanges.
    //  - navegaciones (Tratamiento, EstadoTurno, Presupuesto...): se
    //    resuelven por FK, no se copian desde el DTO.
    // Mapear cada par DTO → Entity detecta igual los type maps faltantes y
    // los type mismatches (AutoMapper tira al construir el plan de mapeo).
    var mapper = CreateMapper();

    mapper.Map<Paciente>(new PacienteDto()).Should().NotBeNull();
    mapper.Map<Profesional>(new ProfesionalDto()).Should().NotBeNull();
    mapper.Map<Turno>(new TurnoDto()).Should().NotBeNull();
    mapper.Map<Presupuesto>(new PresupuestoDto()).Should().NotBeNull();
    mapper.Map<PresupuestoDetalle>(new PresupuestoDetalleDto()).Should().NotBeNull();
    mapper.Map<Pago>(new PagoDto()).Should().NotBeNull();
    mapper.Map<FichaTratamiento>(new FichaTratamientoDto()).Should().NotBeNull();
    mapper.Map<HistoriaClinica>(new HistoriaClinicaDto()).Should().NotBeNull();
    mapper.Map<OdontogramaPieza>(new OdontogramaPiezaDto()).Should().NotBeNull();
    mapper.Map<Ubicacion>(new UbicacionDto()).Should().NotBeNull();
  }

  [Fact]
  public void Paciente_ListToDtoList_MapsEmptyCollection()
  {
    // Reproduce el path exacto de GET /api/paciente/GetAll: el controller
    // mapea la lista completa (aunque no haya un solo registro) con
    // _mapper.Map<List<PacienteDto>>(result.Items). Si el perfil no puede
    // mapear Paciente → PacienteDto, AutoMapper tira al construir el plan
    // de mapeo y el request termina en 400 aunque la tabla esté vacía.
    var mapper = CreateMapper();

    var list = mapper.Map<List<PacienteDto>>(new List<Paciente>());

    list.Should().BeEmpty();
  }
}
