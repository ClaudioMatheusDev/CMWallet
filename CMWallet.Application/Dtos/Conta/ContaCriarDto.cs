using System.ComponentModel.DataAnnotations;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class ContaCriarDto
    {
        [Required(ErrorMessage = "O nome da conta é obrigatório.")]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        public decimal SaldoInicial { get; set; }

        [EnumDataType(typeof(TipoConta), ErrorMessage = "Tipo de conta inválido.")]
        public TipoConta TipoConta { get; set; }
    }
}
