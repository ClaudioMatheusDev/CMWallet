namespace CMWallet.Application.Exceptions;

/// <summary>Dados bem formados que violam uma regra de negócio. Respondido como HTTP 400.</summary>
public class ValidacaoNegocioException : BusinessException
{
    public ValidacaoNegocioException(string message)
        : base(message)
    {
    }
}
