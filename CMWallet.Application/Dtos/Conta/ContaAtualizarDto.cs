using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class ContaAtualizarDto
    {
        public required string Nome { get; set; }
        public decimal SaldoInicial { get; set; }
        public TipoConta TipoConta { get; set; }
    }
}
