using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarTransacaoAsync(Transacao transacao);
        Task<Transacao?> BuscarTransacoesPorIdAsync(int TransacaoId);
        Task<List<Transacao>> ListarTodasTransacoes();
        void DeletarTransacao(Transacao transacao);
        void AtualizarTransacao(Transacao transacao);
        Task SalvarAlteracoesAsync();

    }
}
