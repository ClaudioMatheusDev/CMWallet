using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class CategoriaResponseDto
    {
        public int CategoriaId { get; set; }
        public required string Nome { get; set; }
        public Tipo Tipo { get; set; }
    }
}
