namespace CMWallet.Application.Dtos
{
    public class MetaResponseDto
    {
        public int MetaId { get; set; }
        public decimal ValorMeta { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime DataMeta { get; set; }
        public int ContaId { get; set; }
    }
}
