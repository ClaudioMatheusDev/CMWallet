using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface IContaService
    {
        Task<int> CriarContaAsync(ContaCriarDto dto);
        Task<ContaResponseDto> BuscarContaPorIdAsync(int contaId);
        Task<List<ContaResponseDto>> ListarContasAsync();
        Task ApagarContaAsync(int contaId);
        Task AtualizarContaAsync(int contaId, ContaAtualizarDto dto);
    }
}
