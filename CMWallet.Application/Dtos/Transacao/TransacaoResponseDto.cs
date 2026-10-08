using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class TransacaoResponseDto
    {
        public int TransacaoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public Tipo Tipo { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public int ContaId { get; set; }
        public string ContaNome { get; set; } = string.Empty;
        public bool Pago { get; set; }
    }
}
