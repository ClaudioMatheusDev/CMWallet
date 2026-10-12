using System.ComponentModel.DataAnnotations;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class CategoriaCriarDto
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        [EnumDataType(typeof(Tipo), ErrorMessage = "Tipo inválido.")]
        public Tipo Tipo { get; set; }
    }
}
