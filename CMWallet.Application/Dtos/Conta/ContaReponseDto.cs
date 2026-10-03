using CMWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMWallet.Application.Dtos
{
    public class ContaReponseDto
    {
        public int ContaId { get; set; }
        public required string Nome { get; set; }
        public decimal SaldoInicial { get; set; }
        public TipoConta TipoConta { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
