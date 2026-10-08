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
        _contas.BuscarContaPorIdAsync(1).Returns(new Conta { ContaId = 1, Nome = "Corrente" });
        _service = new MetaService(_metas, _contas);
    }

    [Fact]
    public async Task Buscar_Inexistente_LancaMetaNaoEncontrada_ParaResponderComo404()
    {
        var ex = await Assert.ThrowsAsync<MetaNaoEncontradaException>(() => _service.BuscarMetaPorIdAsync(3));

        Assert.IsAssignableFrom<RegistroNaoEncontradoException>(ex);
    }

    [Fact]
    public async Task Criar_ContaInexistente_LancaContaNaoEncontrada()
    {
        var dto = new MetaCriarDto { ValorMeta = 100, DataMeta = DateTime.UtcNow, ContaId = 42 };

        await Assert.ThrowsAsync<ContaNaoEncontradaException>(() => _service.CriarMetaAsync(dto));
        await _metas.DidNotReceive().SalvarAlteracoesAsync();
    }

    [Fact]
    public async Task Criar_Valida_PersisteMeta()
    {
        var dto = new MetaCriarDto { ValorMeta = 100, ValorAtual = 10, DataMeta = DateTime.UtcNow, ContaId = 1 };

        await _service.CriarMetaAsync(dto);

        await _metas.Received(1).AdicionarMetaAsync(Arg.Is<MetaFinanceira>(m => m.ValorMeta == 100 && m.ContaId == 1));
        await _metas.Received(1).SalvarAlteracoesAsync();
    }

    [Fact]
    public async Task Apagar_Inexistente_LancaMetaNaoEncontrada()
    {
        await Assert.ThrowsAsync<MetaNaoEncontradaException>(() => _service.ApagarMetaAsync(9));
    }
}
