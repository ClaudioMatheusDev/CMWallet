using CMWallet.Application.Dtos;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarTransacaoAsync(Transacao transacao);
        Task<Transacao?> BuscarTransacaoPorIdAsync(int transacaoId);
        Task<(List<Transacao> Itens, int Total)> ListarTransacoesAsync(TransacaoFiltroDto filtro);
        void DeletarTransacao(Transacao transacao);
        Task SalvarAlteracoesAsync();
    }
}
