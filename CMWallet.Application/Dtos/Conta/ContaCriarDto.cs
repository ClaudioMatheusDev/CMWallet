using CMWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMWallet.Application.Dtos
{
    public class ContaCriarDto
    {
        public required string Nome { get; set; }
        public decimal SaldoInicial { get; set; }
        public TipoConta TipoConta { get; set; }
    }
}
