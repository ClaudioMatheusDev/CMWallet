using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface IMetaRepository
    {
        Task AdicionarMetaAsync(MetaFinanceira meta);
        Task<MetaFinanceira?> BuscarMetaPorIdAsync(int metaId);
        Task<List<MetaFinanceira>> ListarTodasMetas();
        void DeletarMeta(MetaFinanceira meta);
        Task SalvarAlteracoesAsync();
    }
}
