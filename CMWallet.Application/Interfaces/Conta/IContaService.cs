using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface IContaService
    {
        Task<int> CriarContaAsync(ContaCriarDto dto, CancellationToken cancellationToken = default);
        Task<ContaResponseDto> BuscarContaPorIdAsync(int contaId, CancellationToken cancellationToken = default);
        Task<List<ContaResponseDto>> ListarContasAsync(CancellationToken cancellationToken = default);
        Task ApagarContaAsync(int contaId, CancellationToken cancellationToken = default);
        Task AtualizarContaAsync(int contaId, ContaAtualizarDto dto, CancellationToken cancellationToken = default);
    }
}
