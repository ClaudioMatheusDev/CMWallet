using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using CMWallet.Domain.Entities;
using CMWallet.Domain.Enums;
using NSubstitute;

namespace CMWallet.Tests.Services;

public class CategoriaServiceTests
{
    private readonly ICategoriaRepository _categorias = Substitute.For<ICategoriaRepository>();
    private readonly CategoriaService _service;
    private readonly Categoria _mercado = new() { CategoriaId = 4, Nome = "Mercado", Tipo = Tipo.Saida };

    public CategoriaServiceTests()
    {
        _categorias.BuscarCategoriaParaEdicaoAsync(4, Arg.Any<CancellationToken>()).Returns(_mercado);
        _service = new CategoriaService(_categorias);
    }

    [Fact]
    public async Task Apagar_EmUso_LancaConflitoComQuantidade()
    {
        _categorias.ContarTransacoesAsync(4, Arg.Any<CancellationToken>()).Returns(2);

        var ex = await Assert.ThrowsAsync<ConflitoNegocioException>(() => _service.ApagarCategoriaAsync(4));

        Assert.Contains("2 transações", ex.Message);
        _categorias.DidNotReceive().DeletarCategoria(Arg.Any<Categoria>());
    }

    [Fact]
    public async Task Apagar_SemUso_ExcluiCategoria()
    {
        await _service.ApagarCategoriaAsync(4);

        _categorias.Received(1).DeletarCategoria(_mercado);
        await _categorias.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Atualizar_TipoDeCategoriaEmUso_LancaConflito()
    {
        _categorias.ContarTransacoesAsync(4, Arg.Any<CancellationToken>()).Returns(1);

        await Assert.ThrowsAsync<ConflitoNegocioException>(() =>
            _service.AtualizarCategoriaAsync(4, new CategoriaAtualizarDto { Nome = "Mercado", Tipo = Tipo.Entrada }));

        Assert.Equal(Tipo.Saida, _mercado.Tipo);
    }

    [Fact]
    public async Task Atualizar_SoONome_DeCategoriaEmUso_Permite()
    {
        _categorias.ContarTransacoesAsync(4, Arg.Any<CancellationToken>()).Returns(5);

        await _service.AtualizarCategoriaAsync(4, new CategoriaAtualizarDto { Nome = " Supermercado ", Tipo = Tipo.Saida });

        Assert.Equal("Supermercado", _mercado.Nome);
        await _categorias.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }
}
