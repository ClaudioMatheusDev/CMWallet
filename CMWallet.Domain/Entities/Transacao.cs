using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Transacao
    {
        public int TransacaoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public Tipo Tipo { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public int ContaId { get; set; }
        public Conta Conta { get; set; } = null!;
        public bool Pago { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
