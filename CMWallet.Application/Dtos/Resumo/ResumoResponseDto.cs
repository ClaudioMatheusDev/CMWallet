using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    public class ResumoResponseDto
    {
        public DateOnly DataInicio { get; init; }
        public DateOnly DataFim { get; init; }

        /// <summary>Soma de todas as entradas do período, pagas ou não.</summary>
        public decimal TotalEntradas { get; init; }

        /// <summary>Soma de todas as saídas do período, pagas ou não.</summary>
        public decimal TotalSaidas { get; init; }

        public decimal Resultado => TotalEntradas - TotalSaidas;

        public int QuantidadeEntradas { get; init; }
        public int QuantidadeSaidas { get; init; }

        /// <summary>Entradas ainda não recebidas.</summary>
        public decimal EntradasPendentes { get; init; }
        public int QuantidadeEntradasPendentes { get; init; }

        /// <summary>Saídas ainda não pagas.</summary>
        public decimal SaidasPendentes { get; init; }
        public int QuantidadeSaidasPendentes { get; init; }

        /// <summary>Totais por categoria, ordenados por tipo e valor decrescente.</summary>
        public required IReadOnlyList<ResumoCategoriaDto> Categorias { get; init; }
    }

    public class ResumoCategoriaDto
    {
        public int CategoriaId { get; init; }
        public required string Nome { get; init; }
        public Tipo Tipo { get; init; }
        public decimal Total { get; init; }
        public int Quantidade { get; init; }

        /// <summary>Participação no total do mesmo tipo (entradas ou saídas), de 0 a 100.</summary>
        public decimal Percentual { get; init; }
    }
}
