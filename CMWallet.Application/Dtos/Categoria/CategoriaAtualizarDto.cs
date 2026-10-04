using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class CategoriaAtualizarDto
    {
        public required string Nome { get; set; }
        public Tipo Tipo { get; set; }
    }
}
