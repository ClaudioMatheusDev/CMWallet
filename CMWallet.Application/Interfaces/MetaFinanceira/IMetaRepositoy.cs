using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface IMetaRepository
    {
        Task AdicionarMetaAsync(MetaFinanceira meta);
        Task<MetaFinanceira?> BuscarMetasPorIdAsync(int MetaId);
        Task<List<MetaFinanceira>> ListarTodasMetas();
        void DeletarMeta(MetaFinanceira meta);
        void AtualizarMeta(MetaFinanceira meta);
        Task SalvarAlteracoesAsync();

    }
}
