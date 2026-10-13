namespace CMWallet.Application.Exceptions;

/// <summary>Operação incompatível com o estado atual dos dados. Respondido como HTTP 409.</summary>
public class ConflitoNegocioException : BusinessException
{
    public ConflitoNegocioException(string message)
        : base(message)
    {
    }
}
