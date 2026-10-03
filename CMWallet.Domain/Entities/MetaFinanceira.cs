using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMWallet.Domain.Entities
{
    public class MetaFinanceira
    {
        [Key]
        public int MetaId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da meta deve ser maior que zero.")]
        public decimal ValorMeta { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor atual não pode ser negativo.")]
        public decimal ValorAtual { get; set; }

        [Required]
        public DateTime DataMeta { get; set; }

        [Required]
        public int ContaId { get; set; }
        public Conta? Conta { get; set; }
    }
}
