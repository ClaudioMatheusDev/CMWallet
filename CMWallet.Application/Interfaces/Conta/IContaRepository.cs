using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface IContaRepository
    {
        Task AdicionarContaAsync(Conta conta);
        Task<Conta?> BuscarContaPorIdAsync(int contaId);
        Task<List<Conta>> ListarTodasContas();
        void DeletarConta(Conta conta);
        Task SalvarAlteracoesAsync();
    }
}
