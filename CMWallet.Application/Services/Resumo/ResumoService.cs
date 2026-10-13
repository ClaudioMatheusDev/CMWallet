using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Services
{
    public class ResumoService : IResumoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IContaRepository _contaRepository;
        private readonly TimeProvider _timeProvider;

        public ResumoService(
            ITransacaoRepository transacaoRepository,
            IContaRepository contaRepository,
            TimeProvider timeProvider)
        {
            _transacaoRepository = transacaoRepository;
            _contaRepository = contaRepository;
            _timeProvider = timeProvider;
        }

        public async Task<ResumoResponseDto> ObterResumoAsync(ResumoFiltroDto filtro, CancellationToken cancellationToken = default)
        {
            var (inicio, fim) = DefinirPeriodo(filtro);

            if (filtro.ContaId is int contaId && !await _contaRepository.ExisteContaAsync(contaId, cancellationToken))
            {
                throw new ContaNaoEncontradaException(contaId);
            }

            var grupos = await _transacaoRepository.AgruparPorCategoriaAsync(inicio, fim, filtro.ContaId, cancellationToken);

            decimal Total(Tipo tipo, bool? pago = null) =>
                grupos.Where(g => g.Tipo == tipo && (pago is null || g.Pago == pago)).Sum(g => g.Total);

            int Quantidade(Tipo tipo, bool? pago = null) =>
                grupos.Where(g => g.Tipo == tipo && (pago is null || g.Pago == pago)).Sum(g => g.Quantidade);

            var totalEntradas = Total(Tipo.Entrada);
            var totalSaidas = Total(Tipo.Saida);

            var categorias = grupos
                .GroupBy(g => (g.CategoriaId, g.Nome, g.Tipo))
                .Select(g =>
                {
                    var total = g.Sum(x => x.Total);
                    var totalDoTipo = g.Key.Tipo == Tipo.Entrada ? totalEntradas : totalSaidas;
                    return new ResumoCategoriaDto
                    {
                        CategoriaId = g.Key.CategoriaId,
                        Nome = g.Key.Nome,
                        Tipo = g.Key.Tipo,
                        Total = total,
                        Quantidade = g.Sum(x => x.Quantidade),
                        Percentual = totalDoTipo == 0 ? 0 : Math.Round(total / totalDoTipo * 100, 2)
                    };
                })
                .OrderBy(c => c.Tipo)
                .ThenByDescending(c => c.Total)
                .ThenBy(c => c.Nome)
                .ToList();

            return new ResumoResponseDto
            {
                DataInicio = inicio,
                DataFim = fim,
                TotalEntradas = totalEntradas,
                TotalSaidas = totalSaidas,
                QuantidadeEntradas = Quantidade(Tipo.Entrada),
                QuantidadeSaidas = Quantidade(Tipo.Saida),
                EntradasPendentes = Total(Tipo.Entrada, pago: false),
                QuantidadeEntradasPendentes = Quantidade(Tipo.Entrada, pago: false),
                SaidasPendentes = Total(Tipo.Saida, pago: false),
                QuantidadeSaidasPendentes = Quantidade(Tipo.Saida, pago: false),
                Categorias = categorias
            };
        }

        /// <summary>Sem datas informadas, usa o mês corrente; com só uma delas, completa com o mês correspondente.</summary>
        private (DateOnly Inicio, DateOnly Fim) DefinirPeriodo(ResumoFiltroDto filtro)
        {
            var referencia = filtro.DataInicio
                ?? filtro.DataFim
                ?? DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

            var inicio = filtro.DataInicio ?? new DateOnly(referencia.Year, referencia.Month, 1);
            var fim = filtro.DataFim ?? new DateOnly(inicio.Year, inicio.Month, 1).AddMonths(1).AddDays(-1);

            if (fim < inicio)
            {
                throw new ValidacaoNegocioException("A data final deve ser igual ou posterior à data inicial.");
            }

            return (inicio, fim);
        }
    }
}
