using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Services;
using OdontoSoft.Domain.Models;
using Xunit;

namespace OdontoSoft.UnitTests.Services;

/// <summary>
/// Tests de LocalidadServices: la sincronización de localidades contra la
/// API georef de datos.gob.ar.
///
/// La parte que más puede fallar es el mapeo (resolver la provincia por
/// nombre y deduplicar), así que se cubre con un HttpClient falso que
/// devuelve un catálogo fijo: sin base de datos y sin llamadas a la red.
/// </summary>
public class LocalidadServicesTests
{
  private static LocalidadServices CreateService(
      string json,
      Mock<ILocalidadRepository> localidadRepo,
      Mock<IProvinciaRepository> provinciaRepo,
      HttpStatusCode status = HttpStatusCode.OK)
  {
    var uow = new Mock<IUnitOfWork>();
    uow.Setup(u => u.LocalidadRepository).Returns(localidadRepo.Object);
    uow.Setup(u => u.ProvinciaRepository).Returns(provinciaRepo.Object);

    var httpClient = new HttpClient(new FakeHttpHandler(json, status));

    return new LocalidadServices(
        NullLogger<LocalidadServices>.Instance,
        uow.Object,
        new Mock<ICurrentUser>().Object,
        httpClient);
  }

  private static Mock<ILocalidadRepository> LocalidadRepo(params Localidad[] existentes)
  {
    var repo = new Mock<ILocalidadRepository>();
    repo.Setup(r => r.All()).Returns(existentes.AsQueryable());
    repo.Setup(r => r.GetAllEnabled()).Returns(existentes.AsQueryable());
    repo.Setup(r => r.BulkInsert(It.IsAny<IEnumerable<Localidad>>()))
        .Returns((IEnumerable<Localidad> l) => l.Count());
    return repo;
  }

  private static Mock<IProvinciaRepository> ProvinciaRepo(params (int Id, string Descripcion)[] provincias)
  {
    var repo = new Mock<IProvinciaRepository>();
    repo.Setup(r => r.GetAllEnabled())
        .Returns(provincias.Select(p => new Provincia { Id = p.Id, Descripcion = p.Descripcion }).AsQueryable());
    return repo;
  }

  private static string Catalogo(params (string Nombre, string Provincia)[] localidades)
  {
    var items = string.Join(",", localidades.Select(l =>
      $"{{\"nombre\":\"{l.Nombre}\",\"provincia\":{{\"id\":\"00\",\"nombre\":\"{l.Provincia}\"}}}}"));

    return $"{{\"cantidad\":{localidades.Length},\"total\":{localidades.Length},\"localidades\":[{items}]}}";
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_InsertaLasQueNoExisten()
  {
    var localidadRepo = LocalidadRepo(
      new Localidad { Descripcion = "Rosario", ProvinciaId = 2 });
    var provinciaRepo = ProvinciaRepo((1, "Ciudad Autónoma de Buenos Aires"), (2, "Buenos Aires"));

    var service = CreateService(
      Catalogo(("Rosario", "Buenos Aires"), ("La Plata", "Buenos Aires")),
      localidadRepo, provinciaRepo);

    var resultado = await service.SincronizarLocalidadesAsync();

    resultado.TotalFuente.Should().Be(2);
    resultado.YaExistentes.Should().Be(1);
    resultado.AInsertar.Should().Be(1);
    resultado.Insertadas.Should().Be(1);
    resultado.SoloSimular.Should().BeFalse();

    localidadRepo.Verify(r => r.BulkInsert(It.Is<IEnumerable<Localidad>>(l => l.Count() == 1)), Times.Once);
    localidadRepo.Verify(r => r.BulkInsert(It.Is<IEnumerable<Localidad>>(
        l => l.First().Descripcion == "La Plata" && l.First().ProvinciaId == 2)), Times.Once);
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_DeduplicaDentroDelLote()
  {
    // La API trae 162 localidades repetidas: mismo nombre en la misma
    // provincia pero en departamentos distintos.
    var localidadRepo = LocalidadRepo();
    var provinciaRepo = ProvinciaRepo((2, "Buenos Aires"));

    var service = CreateService(
      Catalogo(("Paso del Durazno", "Buenos Aires"), ("Paso del Durazno", "Buenos Aires")),
      localidadRepo, provinciaRepo);

    var resultado = await service.SincronizarLocalidadesAsync();

    resultado.TotalFuente.Should().Be(2);
    resultado.AInsertar.Should().Be(1);
    resultado.YaExistentes.Should().Be(1);
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_MatcheaLaProvinciaSinAcentosNiMayusculas()
  {
    var localidadRepo = LocalidadRepo();
    var provinciaRepo = ProvinciaRepo((6, "Córdoba"));

    var service = CreateService(
      Catalogo(("Villa María", "cordoba")),
      localidadRepo, provinciaRepo);

    var resultado = await service.SincronizarLocalidadesAsync();

    resultado.SinProvincia.Should().Be(0);
    resultado.AInsertar.Should().Be(1);
    localidadRepo.Verify(r => r.BulkInsert(It.Is<IEnumerable<Localidad>>(
        l => l.First().ProvinciaId == 6)), Times.Once);
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_ProvinciaDesconocida_LaOmiteYLaReporta()
  {
    var localidadRepo = LocalidadRepo();
    var provinciaRepo = ProvinciaRepo((2, "Buenos Aires"));

    var service = CreateService(
      Catalogo(("Rosario", "Buenos Aires"), ("X", "Provincia Inventada")),
      localidadRepo, provinciaRepo);

    var resultado = await service.SincronizarLocalidadesAsync();

    resultado.SinProvincia.Should().Be(1);
    resultado.AInsertar.Should().Be(1);
    resultado.ProvinciasNoEncontradas.Should().ContainSingle().Which.Should().Be("Provincia Inventada");
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_Preview_NoEscribeNada()
  {
    var localidadRepo = LocalidadRepo();
    var provinciaRepo = ProvinciaRepo((2, "Buenos Aires"));

    var service = CreateService(
      Catalogo(("Rosario", "Buenos Aires"), ("La Plata", "Buenos Aires")),
      localidadRepo, provinciaRepo);

    var resultado = await service.SincronizarLocalidadesAsync(soloSimular: true);

    resultado.SoloSimular.Should().BeTrue();
    resultado.AInsertar.Should().Be(2);
    resultado.Insertadas.Should().Be(0);

    localidadRepo.Verify(r => r.BulkInsert(It.IsAny<IEnumerable<Localidad>>()), Times.Never);
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_YaSincronizada_NoEscribeNada()
  {
    // La tabla ya tiene las tres localidades: una segunda corrida no
    // debe tocar la base.
    var localidadRepo = LocalidadRepo(
      new Localidad { Descripcion = "Rosario", ProvinciaId = 2 },
      new Localidad { Descripcion = "La Plata", ProvinciaId = 2 });
    var provinciaRepo = ProvinciaRepo((2, "Buenos Aires"));

    var service = CreateService(
      Catalogo(("Rosario", "Buenos Aires"), ("La Plata", "Buenos Aires")),
      localidadRepo, provinciaRepo);

    var resultado = await service.SincronizarLocalidadesAsync();

    resultado.AInsertar.Should().Be(0);
    resultado.YaExistentes.Should().Be(2);
    localidadRepo.Verify(r => r.BulkInsert(It.IsAny<IEnumerable<Localidad>>()), Times.Never);
  }

  [Fact]
  public async Task SincronizarLocalidadesAsync_ErrorDeLaApi_PropagaLaExcepcion()
  {
    var localidadRepo = LocalidadRepo();
    var provinciaRepo = ProvinciaRepo((2, "Buenos Aires"));

    var service = CreateService(
      "{}", localidadRepo, provinciaRepo, HttpStatusCode.ServiceUnavailable);

    var ex = await Assert.ThrowsAsync<Exception>(() => service.SincronizarLocalidadesAsync());

    ex.Message.Should().Contain("API de localidades");
    localidadRepo.Verify(r => r.BulkInsert(It.IsAny<IEnumerable<Localidad>>()), Times.Never);
  }

  /// <summary>Handler que devuelve un JSON fijo, sin tocar la red.</summary>
  private sealed class FakeHttpHandler : HttpMessageHandler
  {
    private readonly string _json;
    private readonly HttpStatusCode _status;

    public FakeHttpHandler(string json, HttpStatusCode status)
    {
      _json = json;
      _status = status;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
      return Task.FromResult(new HttpResponseMessage(_status)
      {
        Content = new StringContent(_json, Encoding.UTF8, "application/json")
      });
    }
  }
}
