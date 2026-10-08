using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public required string Nome { get; set; }
        public Tipo Tipo { get; set; }
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
