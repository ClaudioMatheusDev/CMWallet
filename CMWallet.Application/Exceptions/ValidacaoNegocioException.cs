namespace CMWallet.Application.Exceptions;

public class ValidacaoNegocioException : BusinessException
{
    public ValidacaoNegocioException(string message)
        : base(message)
    {
    }
}
