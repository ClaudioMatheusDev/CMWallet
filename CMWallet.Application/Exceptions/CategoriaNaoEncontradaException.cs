namespace CMWallet.Application.Exceptions;

public class CategoriaNaoEncontradaException : RegistroNaoEncontradoException
{
    public CategoriaNaoEncontradaException(int categoriaId)
        : base($"Categoria {categoriaId} não encontrada.")
    {
    }
}
