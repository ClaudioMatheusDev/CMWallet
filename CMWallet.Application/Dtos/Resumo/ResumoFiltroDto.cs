namespace CMWallet.Application.Dtos
{
    public class ResumoFiltroDto
    {
        /// <summary>Data inicial, inclusiva. Padrão: primeiro dia do mês da data final ou do mês atual.</summary>
        public DateOnly? DataInicio { get; set; }

        /// <summary>Data final, inclusiva. Padrão: último dia do mês da data inicial.</summary>
        public DateOnly? DataFim { get; set; }

        /// <summary>Restringe o resumo a uma conta.</summary>
        public int? ContaId { get; set; }
    }
}
