using System.ComponentModel.DataAnnotations;

namespace CMWallet.Application.Dtos
{
    public class MetaAtualizarDto
    {
        [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "O valor da meta deve ser maior que zero.")]
        public decimal ValorMeta { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999", ErrorMessage = "O valor atual não pode ser negativo.")]
        public decimal ValorAtual { get; set; }

        [Required(ErrorMessage = "A data da meta é obrigatória.")]
        public DateTime? DataMeta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A conta é obrigatória.")]
        public int ContaId { get; set; }
    }
}
