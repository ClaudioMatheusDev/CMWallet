using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using CMWallet.Domain.Enums;
using NSubstitute;

namespace CMWallet.Tests.Services;

public class ResumoServiceTests
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 12, 15, 0, 0, TimeSpan.Zero);

    private readonly ITransacaoRepository _transacoes = Substitute.For<ITransacaoRepository>();
    private readonly IContaRepository _contas = Substitute.For<IContaRepository>();
    private readonly ResumoService _service;

    public ResumoServiceTests()
    {
        _service = new ResumoService(_transacoes, _contas, new FakeTimeProvider(Agora));
    }

    [Fact]
    public async Task SemDatas_UsaMesCorrente()
    {
        _transacoes.AgruparPorCategoriaAsync(default, default, null, default).ReturnsForAnyArgs([]);

        var resumo = await _service.ObterResumoAsync(new ResumoFiltroDto());

        Assert.Equal(new DateOnly(2026, 10, 1), resumo.DataInicio);
        Assert.Equal(new DateOnly(2026, 10, 31), resumo.DataFim);
        await _transacoes.Received(1).AgruparPorCategoriaAsync(
            new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 31), null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SomaTotaisPendenciasECategorias()
    {
        _transacoes.AgruparPorCategoriaAsync(default, default, null, default).ReturnsForAnyArgs(
        [
            new TotalPorCategoria(1, "Salário", Tipo.Entrada, Pago: true, 5000, 1),
            new TotalPorCategoria(2, "Mercado", Tipo.Saida, Pago: true, 600, 3),
            new TotalPorCategoria(2, "Mercado", Tipo.Saida, Pago: false, 200, 1),
            new TotalPorCategoria(3, "Aluguel", Tipo.Saida, Pago: false, 1200, 1),
        ]);

        var resumo = await _service.ObterResumoAsync(new ResumoFiltroDto());

        Assert.Equal(5000, resumo.TotalEntradas);
        Assert.Equal(2000, resumo.TotalSaidas);
        Assert.Equal(3000, resumo.Resultado);
        Assert.Equal(1400, resumo.SaidasPendentes);
        Assert.Equal(2, resumo.QuantidadeSaidasPendentes);
        Assert.Equal(5, resumo.QuantidadeSaidas);

        Assert.Collection(
            resumo.Categorias,
            c => Assert.Equal(("Salário", 5000m, 100m), (c.Nome, c.Total, c.Percentual)),
            c => Assert.Equal(("Aluguel", 1200m, 60m), (c.Nome, c.Total, c.Percentual)),
            c => Assert.Equal(("Mercado", 800m, 40m, 4), (c.Nome, c.Total, c.Percentual, c.Quantidade)));
    }

    [Fact]
    public async Task PeriodoInvertido_LancaValidacao()
    {
        var filtro = new ResumoFiltroDto { DataInicio = new DateOnly(2026, 10, 10), DataFim = new DateOnly(2026, 10, 1) };

        await Assert.ThrowsAsync<ValidacaoNegocioException>(() => _service.ObterResumoAsync(filtro));
    }

    [Fact]
    public async Task ContaInexistente_LancaContaNaoEncontrada()
    {
        await Assert.ThrowsAsync<ContaNaoEncontradaException>(() =>
            _service.ObterResumoAsync(new ResumoFiltroDto { ContaId = 99 }));
    }
}
