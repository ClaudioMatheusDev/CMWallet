using System.ComponentModel.DataAnnotations;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class TransacaoFiltroDto
    {
        public const int TamanhoPaginaMaximo = 100;

        public int? ContaId { get; set; }
        public int? CategoriaId { get; set; }
        public Tipo? Tipo { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        [Range(1, int.MaxValue)]
        public int Pagina { get; set; } = 1;

        [Range(1, TamanhoPaginaMaximo)]
        public int TamanhoPagina { get; set; } = 20;
    }
}
