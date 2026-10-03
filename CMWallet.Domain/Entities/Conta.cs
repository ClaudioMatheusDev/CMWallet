using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Conta
    {
        [Key]
        public int ContaId { get; set; }

        [Required]
        [StringLength(200)]
        public required string Nome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoInicial { get; set; }

        [Required]
        public TipoConta TipoConta { get; set; }
        public DateTime DataCriacao { get; set; } 
        public DateTime DataAtualizacao { get; set; }
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
        public ICollection<MetaFinanceira> MetasFinanceiras { get; set; } = new List<MetaFinanceira>();
    }
}
