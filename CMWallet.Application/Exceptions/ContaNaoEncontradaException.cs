namespace CMWallet.Application.Exceptions;

public class ContaNaoEncontradaException : RegistroNaoEncontradoException
{
    public ContaNaoEncontradaException(int contaId)
        : base($"Conta com Id {contaId}")
    {
    }
}
