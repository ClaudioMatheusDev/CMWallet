using CMWallet.Domain.Enums;

namespace CMWallet.Domain.Entities
{
    public class Transacao
    {
        public int IDTransacao { get; set; }
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime Data { get; set; }
        public Tipo Tipo { get; set; }
        public int IDCategoria { get; set; }
        public int IDConta { get; set; }
        public bool Pago    { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow.AddHours(-3);
    }
}