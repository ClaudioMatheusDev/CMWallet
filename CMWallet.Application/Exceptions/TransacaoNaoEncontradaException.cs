namespace CMWallet.Application.Exceptions;

public class TransacaoNaoEncontradaException : RegistroNaoEncontradoException
{
    public TransacaoNaoEncontradaException(int transacaoId)
        : base($"Transação com Id {transacaoId}")
    {
    }
}
