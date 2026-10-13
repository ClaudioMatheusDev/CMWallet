using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using CMWallet.Domain.Entities;
using NSubstitute;

namespace CMWallet.Tests.Services;

public class MetaServiceTests
{
    private readonly IMetaRepository _metas = Substitute.For<IMetaRepository>();
    private readonly IContaRepository _contas = Substitute.For<IContaRepository>();
    private readonly MetaService _service;

    public MetaServiceTests()
    {
        _contas.ExisteContaAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _service = new MetaService(_metas, _contas);
    }

    [Fact]
    public async Task Buscar_Inexistente_LancaMetaNaoEncontrada_ParaResponderComo404()
    {
        var ex = await Assert.ThrowsAsync<MetaNaoEncontradaException>(() => _service.BuscarMetaPorIdAsync(3));

        Assert.IsAssignableFrom<RegistroNaoEncontradoException>(ex);
        Assert.Equal("Meta 3 não encontrada.", ex.Message);
    }

    [Fact]
    public async Task Criar_ContaInexistente_LancaContaNaoEncontrada()
    {
        var dto = new MetaCriarDto { ValorMeta = 100, DataMeta = new DateOnly(2027, 1, 1), ContaId = 42 };

        await Assert.ThrowsAsync<ContaNaoEncontradaException>(() => _service.CriarMetaAsync(dto));
        await _metas.DidNotReceive().SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Criar_Valida_PersisteMeta()
    {
        var dto = new MetaCriarDto { ValorMeta = 100, ValorAtual = 10, DataMeta = new DateOnly(2027, 1, 1), ContaId = 1 };

        await _service.CriarMetaAsync(dto);

        await _metas.Received(1).AdicionarMetaAsync(
            Arg.Is<MetaFinanceira>(m => m.ValorMeta == 100 && m.ContaId == 1 && m.DataMeta == new DateOnly(2027, 1, 1)),
            Arg.Any<CancellationToken>());
        await _metas.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Apagar_Inexistente_LancaMetaNaoEncontrada()
    {
        await Assert.ThrowsAsync<MetaNaoEncontradaException>(() => _service.ApagarMetaAsync(9));
    }
}
