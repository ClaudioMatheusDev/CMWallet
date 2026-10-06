using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface ITransacaoService
    {
        Task<int> CriarTransacaoAsync(TransacaoCriarDto dto);
        Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId);
        Task<List<TransacaoResponseDto>> ListarTransacoesAsync();
        Task ApagarTransacaoAsync(int TransacaoId);
        Task AtualizarTransacaoAsync(int TransacaoId, TransacaoAtualizarDto dto);
    }
}
