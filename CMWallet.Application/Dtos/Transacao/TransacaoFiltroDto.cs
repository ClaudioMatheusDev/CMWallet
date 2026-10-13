using System.ComponentModel.DataAnnotations;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class TransacaoFiltroDto
    {
        public const int TamanhoPaginaMaximo = 100;

        /// <summary>Limita o deslocamento (pagina × tamanho) para que o cálculo não estoure um int.</summary>
        public const int PaginaMaxima = 1_000_000;

        public int? ContaId { get; set; }

        public int? CategoriaId { get; set; }

        public Tipo? Tipo { get; set; }

        /// <summary>Data inicial, inclusiva.</summary>
        public DateOnly? DataInicio { get; set; }

        /// <summary>Data final, inclusiva.</summary>
        public DateOnly? DataFim { get; set; }

        [Range(1, PaginaMaxima, ErrorMessage = "A página deve estar entre 1 e {2}.")]
        public int Pagina { get; set; } = 1;

        [Range(1, TamanhoPaginaMaximo, ErrorMessage = "O tamanho da página deve estar entre 1 e {2}.")]
        public int TamanhoPagina { get; set; } = 20;
    }
}
