namespace CMWallet.Application.Dtos
{
    public class MetaCriarDto
    {
        public decimal ValorMeta { get; set; }
        public decimal ValorAtual { get; set; }

        public DateTime DataMeta { get; set; }
        public int ContaId { get; set; }
    }
}
