using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class CategoriaCriarDto
    {
        public required string Nome { get; set; }
        public Tipo Tipo { get; set; }
    }
}
