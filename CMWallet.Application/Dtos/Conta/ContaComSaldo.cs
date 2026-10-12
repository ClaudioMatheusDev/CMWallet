using CMWallet.Domain.Entities;

namespace CMWallet.Application.Dtos
{
    /// <summary>Conta lida junto com o saldo atual calculado pelo banco.</summary>
    public sealed record ContaComSaldo(Conta Conta, decimal SaldoAtual);
}
