namespace CMWallet.Domain.Entities
{
    public class MetaFinanceira
    {
        public int MetaId { get; set; }
        public decimal ValorMeta { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime DataMeta { get; set; }
        public int ContaId { get; set; }
        public Conta? Conta { get; set; }
    }
}
