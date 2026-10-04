using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface IContaService
    {
        Task<int> CriarContaAsync(ContaCriarDto dto);
        Task<ContaReponseDto> BuscarContaPorIdAsync(int contaId);
        Task<List<ContaReponseDto>> ListarContasAsync();
        Task ApagarContaAsync (int ContaId);  
        Task AtualizarContaAsync (int ContaId, ContaAtualizarDto dto);
    }
}
