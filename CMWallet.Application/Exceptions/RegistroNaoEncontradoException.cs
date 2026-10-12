namespace CMWallet.Application.Exceptions;

/// <summary>Registro inexistente. Respondido como HTTP 404.</summary>
public abstract class RegistroNaoEncontradoException : BusinessException
{
    protected RegistroNaoEncontradoException(string message)
        : base(message)
    {
    }
}
