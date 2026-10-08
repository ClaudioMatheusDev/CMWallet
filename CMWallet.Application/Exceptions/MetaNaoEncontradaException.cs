namespace CMWallet.Application.Exceptions;

public class MetaNaoEncontradaException : RegistroNaoEncontradoException
{
    public MetaNaoEncontradaException(int metaId)
        : base($"Meta com Id {metaId}")
    {
    }
}
