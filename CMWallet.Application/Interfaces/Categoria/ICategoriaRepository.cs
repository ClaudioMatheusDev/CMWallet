using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface ICategoriaRepository
    {
        Task AdicionarCategoriaAsync(Categoria categoria, CancellationToken cancellationToken = default);

        /// <summary>Categoria rastreada pelo contexto, para alteração ou exclusão.</summary>
        Task<Categoria?> BuscarCategoriaParaEdicaoAsync(int categoriaId, CancellationToken cancellationToken = default);

        Task<Categoria?> BuscarCategoriaPorIdAsync(int categoriaId, CancellationToken cancellationToken = default);

        Task<List<Categoria>> ListarTodasCategoriasAsync(CancellationToken cancellationToken = default);

        Task<int> ContarTransacoesAsync(int categoriaId, CancellationToken cancellationToken = default);

        void DeletarCategoria(Categoria categoria);

        Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    }
}
