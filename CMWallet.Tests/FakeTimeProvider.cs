namespace CMWallet.Tests;

internal sealed class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _agora;

    public FakeTimeProvider(DateTimeOffset agora)
    {
        _agora = agora;
    }

    public override DateTimeOffset GetUtcNow() => _agora;
}
