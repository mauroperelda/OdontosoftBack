using FluentAssertions;
using OdontoSoft.Contracts.Common;
using Xunit;

namespace OdontoSoft.UnitTests.Helpers;

public class PaginationParamsTests
{
  [Fact]
  public void Defaults_Page1_PageSize50()
  {
    var p = new PaginationParams();

    p.Page.Should().Be(1);
    p.PageSize.Should().Be(50);
    p.SearchText.Should().BeNull();
  }

  [Theory]
  [InlineData(0, 50)]
  [InlineData(-1, 50)]
  [InlineData(-100, 50)]
  public void PageSize_NonPositive_FallsBackToDefault(int input, int expected)
  {
    var p = new PaginationParams { PageSize = input };

    p.PageSize.Should().Be(expected);
  }

  [Theory]
  [InlineData(500, 200)]
  [InlineData(1000, 200)]
  [InlineData(int.MaxValue, 200)]
  public void PageSize_AboveMax_CapsAtMaxPageSize(int input, int expected)
  {
    var p = new PaginationParams { PageSize = input };

    p.PageSize.Should().Be(expected);
  }

  [Theory]
  [InlineData(1, 1)]
  [InlineData(50, 50)]
  [InlineData(200, 200)]
  public void PageSize_ValidValue_PassesThrough(int input, int expected)
  {
    var p = new PaginationParams { PageSize = input };

    p.PageSize.Should().Be(expected);
  }
}

public class PagedResultTests
{
  [Fact]
  public void TotalPages_EmptyResult_IsZero()
  {
    var r = new PagedResult<int> { Total = 0, PageSize = 50 };

    r.TotalPages.Should().Be(0);
  }

  [Fact]
  public void TotalPages_PartialPage_CeilingsUp()
  {
    var r = new PagedResult<int> { Total = 51, PageSize = 50 };

    r.TotalPages.Should().Be(2);
  }

  [Fact]
  public void TotalPages_ExactMultiple_FloorNotCeil()
  {
    var r = new PagedResult<int> { Total = 100, PageSize = 50 };

    r.TotalPages.Should().Be(2);
  }

  [Fact]
  public void TotalPages_LargeDataset_CalculatesCorrectly()
  {
    var r = new PagedResult<int> { Total = 1234, PageSize = 50 };

    // 1234 / 50 = 24.68 → ceil = 25
    r.TotalPages.Should().Be(25);
  }
}
