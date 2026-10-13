using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface IMetaRepository
    {
        Task AdicionarMetaAsync(MetaFinanceira meta, CancellationToken cancellationToken = default);

        /// <summary>Meta rastreada pelo contexto, para alteração ou exclusão.</summary>
        Task<MetaFinanceira?> BuscarMetaParaEdicaoAsync(int metaId, CancellationToken cancellationToken = default);

        Task<MetaFinanceira?> BuscarMetaPorIdAsync(int metaId, CancellationToken cancellationToken = default);

        Task<List<MetaFinanceira>> ListarTodasMetasAsync(CancellationToken cancellationToken = default);

        void DeletarMeta(MetaFinanceira meta);

        Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    }
}
