using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface ITransacaoService
    {
        Task<int> CriarTransacaoAsync(TransacaoCriarDto dto, CancellationToken cancellationToken = default);
        Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId, CancellationToken cancellationToken = default);
        Task<PagedResult<TransacaoResponseDto>> ListarTransacoesAsync(TransacaoFiltroDto filtro, CancellationToken cancellationToken = default);
        Task ApagarTransacaoAsync(int transacaoId, CancellationToken cancellationToken = default);
        Task AtualizarTransacaoAsync(int transacaoId, TransacaoAtualizarDto dto, CancellationToken cancellationToken = default);
    }
}
