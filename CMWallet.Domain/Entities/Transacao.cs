using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Transacao
    {
        [Key]
        public int TransacaoId { get; set; }

        [Required]
        [StringLength(300)]
        public string Descricao { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da transação deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public Tipo Tipo { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        [Required]
        public int ContaId { get; set; }
        public Conta Conta { get; set; } = null!;

        public bool Pago { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }
    }
}