using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Conta
    {
        public int ContaId { get; set; }
        public required string Nome { get; set; }
        public decimal SaldoInicial { get; set; }
        public TipoConta TipoConta { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
        public ICollection<MetaFinanceira> MetasFinanceiras { get; set; } = new List<MetaFinanceira>();
    }
}
