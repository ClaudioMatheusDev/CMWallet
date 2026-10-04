using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface ICategoriaRepository
    {
        Task AdicionarCategoriaAsync(Categoria categoria);
        Task<Categoria?> BuscarCategoriaPorIdAsync(int categoriaId);
        Task<List<Categoria>> ListarTodasCategorias();
        void DeletarCategoria(Categoria categoria);
        void AtualizarCategoria(Categoria categoria);
        Task SalvarAlteracoesAsync();
    }
}
