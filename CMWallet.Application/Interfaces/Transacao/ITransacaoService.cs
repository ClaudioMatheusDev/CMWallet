using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface ITransacaoService
    {
        Task<int> CriarTransacaoAsync(TransacaoCriarDto dto);
        Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId);
        Task<PagedResult<TransacaoResponseDto>> ListarTransacoesAsync(TransacaoFiltroDto filtro);
        Task ApagarTransacaoAsync(int transacaoId);
        Task AtualizarTransacaoAsync(int transacaoId, TransacaoAtualizarDto dto);
    }
}
