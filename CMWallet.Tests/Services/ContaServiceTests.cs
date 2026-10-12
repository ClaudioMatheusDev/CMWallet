using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using CMWallet.Domain.Entities;
using NSubstitute;

namespace CMWallet.Tests.Services;

public class ContaServiceTests
{
    private readonly IContaRepository _contas = Substitute.For<IContaRepository>();
    private readonly ContaService _service;
    private readonly Conta _corrente = new() { ContaId = 1, Nome = "Corrente", SaldoInicial = 100 };

    public ContaServiceTests()
    {
        _contas.BuscarContaParaEdicaoAsync(1, Arg.Any<CancellationToken>()).Returns(_corrente);
        _service = new ContaService(_contas, new FakeTimeProvider(DateTimeOffset.UnixEpoch));
    }

    [Fact]
    public async Task Listar_RetornaSaldoAtualCalculado()
    {
        _contas.ListarContasComSaldoAsync(Arg.Any<CancellationToken>())
            .Returns([new ContaComSaldo(_corrente, 350.75m)]);

        var contas = await _service.ListarContasAsync();

        var conta = Assert.Single(contas);
        Assert.Equal(100, conta.SaldoInicial);
        Assert.Equal(350.75m, conta.SaldoAtual);
    }

    [Fact]
    public async Task Apagar_ComTransacoesEMetas_LancaConflitoSemExcluir()
    {
        _contas.ContarVinculosAsync(1, Arg.Any<CancellationToken>()).Returns((3, 1));

        var ex = await Assert.ThrowsAsync<ConflitoNegocioException>(() => _service.ApagarContaAsync(1));

        Assert.Contains("3 transações e 1 meta", ex.Message);
        _contas.DidNotReceive().DeletarConta(Arg.Any<Conta>());
        await _contas.DidNotReceive().SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Apagar_SemVinculos_ExcluiConta()
    {
        _contas.ContarVinculosAsync(1, Arg.Any<CancellationToken>()).Returns((0, 0));

        await _service.ApagarContaAsync(1);

        _contas.Received(1).DeletarConta(_corrente);
        await _contas.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Buscar_Inexistente_LancaContaNaoEncontrada()
    {
        var ex = await Assert.ThrowsAsync<ContaNaoEncontradaException>(() => _service.BuscarContaPorIdAsync(9));
        Assert.Equal("Conta 9 não encontrada.", ex.Message);
    }
}
