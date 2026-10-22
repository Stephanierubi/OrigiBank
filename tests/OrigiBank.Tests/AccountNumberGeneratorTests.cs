using Microsoft.Extensions.Time.Testing;
using OrigiBank.Application.Services;

namespace OrigiBank.Tests;

public class AccountNumberGeneratorTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 6, 18, 10, 0, 0, TimeSpan.Zero);

    private static AccountNumberGenerator CreateGenerator() =>
        new(new FakeTimeProvider(FixedNow));

    [Fact]
    public void Generate_ReturnsExactLengthOf17()
    {
        var number = CreateGenerator().Generate();

        Assert.Equal(17, number.Length);
    }

    [Fact]
    public void Generate_StartsWithAccPrefix()
    {
        var number = CreateGenerator().Generate();

        Assert.StartsWith("ACC-", number);
    }

    [Fact]
    public void Generate_ContainsCreationDate()
    {
        var number = CreateGenerator().Generate();

        Assert.Equal("20260618", number.Substring(4, 8));
    }

    [Fact]
    public void Generate_UsesTodaysDate_WhenUsingSystemClock()
    {
        var generator = new AccountNumberGenerator(TimeProvider.System);

        var number = generator.Generate();

        Assert.Equal(DateTime.UtcNow.ToString("yyyyMMdd"), number.Substring(4, 8));
    }

    [Fact]
    public void Generate_EndsWithFourDigits()
    {
        var number = CreateGenerator().Generate();
        var suffix = number[^4..];

        Assert.Equal(4, suffix.Length);
        Assert.All(suffix, c => Assert.True(char.IsDigit(c)));
    }

    [Fact]
    public void Generate_ProducesDistinctNumbers()
    {
        var generator = CreateGenerator();

        var numbers = Enumerable.Range(0, 1000).Select(_ => generator.Generate()).ToList();

        Assert.Equal(numbers.Count, numbers.Distinct().Count());
    }
}