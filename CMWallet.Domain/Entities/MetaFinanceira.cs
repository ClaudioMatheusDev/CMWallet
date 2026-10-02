namespace CMWallet.Domain.Entities
{
    public class MetaFinanceira
    {
        public int IDMeta {  get; set; }
        public decimal ValorMeta { get; set; }
        public decimal ValorAtual  { get; set; }
        public DateTime DataMeta { get; set; }
    }
}
