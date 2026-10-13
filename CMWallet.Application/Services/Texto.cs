namespace CMWallet.Application.Services;

internal static class Texto
{
    /// <summary>Ex.: <c>Quantidade(1, "meta", "metas")</c> → "1 meta".</summary>
    public static string Quantidade(int quantidade, string singular, string plural) =>
        $"{quantidade} {(quantidade == 1 ? singular : plural)}";
}
