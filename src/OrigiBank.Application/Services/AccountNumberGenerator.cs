using OrigiBank.Application.Abstractions;

namespace OrigiBank.Application.Services;

public sealed class AccountNumberGenerator(TimeProvider clock) : IAccountNumberGenerator
{
    private const string Prefix = "ACC";
    private const int Modulus = 10_000;

    private int _counter = Random.Shared.Next(Modulus);

    public string Generate()
    {
        var date = clock.GetUtcNow().ToString("yyyyMMdd");
        var suffix = (uint)Interlocked.Increment(ref _counter) % Modulus;
        return $"{Prefix}-{date}-{suffix:D4}";
    }
}