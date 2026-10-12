using CMWallet.Application.Dtos;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Interfaces
{
    public interface IContaRepository
    {
        Task AdicionarContaAsync(Conta conta, CancellationToken cancellationToken = default);

        /// <summary>Conta rastreada pelo contexto, para alteração ou exclusão.</summary>
        Task<Conta?> BuscarContaParaEdicaoAsync(int contaId, CancellationToken cancellationToken = default);

        Task<ContaComSaldo?> BuscarContaComSaldoAsync(int contaId, CancellationToken cancellationToken = default);

        Task<List<ContaComSaldo>> ListarContasComSaldoAsync(CancellationToken cancellationToken = default);

        Task<bool> ExisteContaAsync(int contaId, CancellationToken cancellationToken = default);

        Task<(int Transacoes, int Metas)> ContarVinculosAsync(int contaId, CancellationToken cancellationToken = default);

        void DeletarConta(Conta conta);

        Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    }
}
