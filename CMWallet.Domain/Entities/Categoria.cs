using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Categoria
    {
        public int IDCategoria { get; set; }
        public required string Nome { get; set; }
        public Tipo Tipo { get; set; }
    }
}
