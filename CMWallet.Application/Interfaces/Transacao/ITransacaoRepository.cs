using CMWallet.Application.Dtos;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarTransacaoAsync(Transacao transacao, CancellationToken cancellationToken = default);

        /// <summary>Transação rastreada pelo contexto (sem navegações), para alteração ou exclusão.</summary>
        Task<Transacao?> BuscarTransacaoParaEdicaoAsync(int transacaoId, CancellationToken cancellationToken = default);

        /// <summary>Transação somente leitura, com conta e categoria carregadas.</summary>
        Task<Transacao?> BuscarTransacaoPorIdAsync(int transacaoId, CancellationToken cancellationToken = default);

        Task<(List<Transacao> Itens, int Total)> ListarTransacoesAsync(TransacaoFiltroDto filtro, CancellationToken cancellationToken = default);

        /// <summary>Totais do período agrupados por categoria, tipo e situação de pagamento.</summary>
        Task<List<TotalPorCategoria>> AgruparPorCategoriaAsync(
            DateOnly dataInicio,
            DateOnly dataFim,
            int? contaId,
            CancellationToken cancellationToken = default);

        void DeletarTransacao(Transacao transacao);

        Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    }
}
