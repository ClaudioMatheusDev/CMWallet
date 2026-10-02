using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Conta
    {
        public int IDConta { get; set; }
        public required string ContaNome  { get; set; }
        public decimal ValorInicial { get; set; }
        public TipoConta TipoConta { get; set; }
    }
}
