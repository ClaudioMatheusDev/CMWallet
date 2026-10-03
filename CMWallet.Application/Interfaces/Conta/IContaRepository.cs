using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface IContaRepository
    {
        Task AdicionarContaAsync(Conta conta);
        Task<Conta?> BuscarContasPorIdAsync(int ContaId);
        Task<List<Conta>> ListarTodasContas();
        void DeletarConta(Conta conta);
        void AtualizarConta(Conta conta);
        Task SalvarAlteracoesAsync();
 
    }
}
