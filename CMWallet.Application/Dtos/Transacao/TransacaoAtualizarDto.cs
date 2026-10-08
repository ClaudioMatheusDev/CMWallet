using System.ComponentModel.DataAnnotations;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class TransacaoAtualizarDto
    {
        [Required(ErrorMessage = "A descrição da transação é obrigatória.")]
        [StringLength(300)]
        public string Descricao { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "O valor da transação deve ser maior que zero.")]
        public decimal Valor { get; set; }

        /// <summary>Nova data da transação. Quando omitida, a data atual é mantida.</summary>
        public DateTime? Data { get; set; }

        [EnumDataType(typeof(Tipo), ErrorMessage = "Tipo inválido.")]
        public Tipo Tipo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A categoria é obrigatória.")]
        public int CategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A conta é obrigatória.")]
        public int ContaId { get; set; }

        public bool Pago { get; set; }
    }
}
