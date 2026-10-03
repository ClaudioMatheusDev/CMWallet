namespace CMWallet.Application.Exceptions;

public class RegistroNaoEncontradoException : BusinessException
{
    public RegistroNaoEncontradoException(string nomeRegistro)
        : base($"{nomeRegistro} não encontrado.")
    {
    }
}
