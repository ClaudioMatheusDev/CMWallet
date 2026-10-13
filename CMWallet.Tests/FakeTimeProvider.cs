namespace CMWallet.Tests;

/// <summary>Relógio fixo em UTC, para que "hoje" não dependa do fuso da máquina que roda os testes.</summary>
internal sealed class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _agora;

    public FakeTimeProvider(DateTimeOffset agora)
    {
        _agora = agora;
    }

    public override DateTimeOffset GetUtcNow() => _agora;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
