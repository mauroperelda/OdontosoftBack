using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Contracts.Common;
using Xunit;

namespace OdontoSoft.UnitTests.Helpers;

/// <summary>
/// Tests del extension method <see cref="QueryableExtensions.ToPagedResultAsync{T}"/>.
/// Garantiza que el contrato de paginaciÃ³n (total, page, pageSize, items)
/// se mantenga estable en los 4 controllers que lo consumen.
///
/// Usa un <see cref="TestDbContext"/> con EF InMemory porque
/// <c>Enumerable.Range(...).AsQueryable()</c> no implementa
/// <see cref="IAsyncQueryProvider"/>, requerido por CountAsync/ToListAsync.
/// </summary>
public class QueryableExtensionsTests
{
  private static TestDbContext CreateContext(string dbName) =>
    new(new DbContextOptionsBuilder<TestDbContext>()
      .UseInMemoryDatabase(databaseName: dbName)
      .Options);

  [Fact]
  public async Task FirstPage_ReturnsFirstPageSizeItems_AndCorrectTotal()
  {
    using var ctx = CreateContext(nameof(FirstPage_ReturnsFirstPageSizeItems_AndCorrectTotal));
    ctx.Numbers.AddRange(BuildNumbers(1, 100));
    await ctx.SaveChangesAsync();

    var page = await ctx.Numbers.ToPagedResultAsync(new PaginationParams { Page = 1, PageSize = 10 });

    page.Items.Should().HaveCount(10);
    page.Items.Select(n => n.Value).Should().Equal(Enumerable.Range(1, 10));
    page.Total.Should().Be(100);
    page.Page.Should().Be(1);
    page.PageSize.Should().Be(10);
    page.TotalPages.Should().Be(10);
  }

  [Fact]
  public async Task MiddlePage_ReturnsCorrectSlice()
  {
    using var ctx = CreateContext(nameof(MiddlePage_ReturnsCorrectSlice));
    ctx.Numbers.AddRange(BuildNumbers(1, 100));
    await ctx.SaveChangesAsync();

    var page = await ctx.Numbers.ToPagedResultAsync(new PaginationParams { Page = 3, PageSize = 10 });

    page.Items.Select(n => n.Value).Should().Equal(Enumerable.Range(21, 10));
    page.Page.Should().Be(3);
    page.TotalPages.Should().Be(10);
  }

  [Fact]
  public async Task LastPartialPage_ReturnsOnlyRemainingItems()
  {
    using var ctx = CreateContext(nameof(LastPartialPage_ReturnsOnlyRemainingItems));
    ctx.Numbers.AddRange(BuildNumbers(1, 25));
    await ctx.SaveChangesAsync();

    var page = await ctx.Numbers.ToPagedResultAsync(new PaginationParams { Page = 3, PageSize = 10 });

    page.Items.Should().HaveCount(5, "sobran 5 elementos despuÃ©s de 2 pÃ¡ginas completas de 10.");
    page.Items.Select(n => n.Value).Should().Equal(Enumerable.Range(21, 5));
    page.TotalPages.Should().Be(3);
  }

  [Fact]
  public async Task EmptySource_ReturnsEmptyResult_NotNull()
  {
    using var ctx = CreateContext(nameof(EmptySource_ReturnsEmptyResult_NotNull));

    var page = await ctx.Numbers.ToPagedResultAsync(new PaginationParams { Page = 1, PageSize = 10 });

    page.Items.Should().NotBeNull().And.BeEmpty();
    page.Total.Should().Be(0);
    page.TotalPages.Should().Be(0);
  }

  [Fact]
  public async Task PageBeyondLast_ReturnsEmptyItems_ButKeepsTotal()
  {
    using var ctx = CreateContext(nameof(PageBeyondLast_ReturnsEmptyItems_ButKeepsTotal));
    ctx.Numbers.AddRange(BuildNumbers(1, 5));
    await ctx.SaveChangesAsync();

    var page = await ctx.Numbers.ToPagedResultAsync(new PaginationParams { Page = 99, PageSize = 10 });

    page.Items.Should().BeEmpty();
    page.Total.Should().Be(5, "el total debe reflejar el universo, no la pÃ¡gina.");
  }

  [Fact]
  public async Task PageSizeClamping_IsRespected()
  {
    using var ctx = CreateContext(nameof(PageSizeClamping_IsRespected));
    ctx.Numbers.AddRange(BuildNumbers(1, 250));
    await ctx.SaveChangesAsync();

    // PaginationParams acota pageSize a MaxPageSize (200) en el setter.
    var pagination = new PaginationParams { Page = 1, PageSize = 1000 };
    pagination.PageSize.Should().Be(200);

    var page = await ctx.Numbers.ToPagedResultAsync(pagination);

    page.Items.Should().HaveCount(200);
    page.PageSize.Should().Be(200);
  }

  private static IEnumerable<NumberEntity> BuildNumbers(int start, int count) =>
    Enumerable.Range(start, count).Select(i => new NumberEntity { Value = i });

  /// <summary>
  /// Entity con PK que sirve para probar el paginador sin levantar
  /// el modelo de dominio entero.
  /// </summary>
  private class NumberEntity
  {
    public int Id { get; set; }
    public int Value { get; set; }
  }

  /// <summary>
  /// DbContext mÃ­nimo con un Ãºnico <see cref="DbSet{T}"/> de entity.
  /// Suficiente para ejercitar el paginador sin levantar todo el
  /// modelo de dominio.
  /// </summary>
  private class TestDbContext : DbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<NumberEntity> Numbers => Set<NumberEntity>();
  }
}
