using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using CMWallet.Domain.Entities;
using CMWallet.Domain.Enums;
using NSubstitute;

namespace CMWallet.Tests.Services;

public class TransacaoServiceTests
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 8, 12, 0, 0, TimeSpan.Zero);

    private readonly ITransacaoRepository _transacoes = Substitute.For<ITransacaoRepository>();
    private readonly IContaRepository _contas = Substitute.For<IContaRepository>();
    private readonly ICategoriaRepository _categorias = Substitute.For<ICategoriaRepository>();
    private readonly TransacaoService _service;

    public TransacaoServiceTests()
    {
        _contas.ExisteContaAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _categorias.BuscarCategoriaPorIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new Categoria { CategoriaId = 2, Nome = "Salário", Tipo = Tipo.Entrada });
        _categorias.BuscarCategoriaPorIdAsync(3, Arg.Any<CancellationToken>())
            .Returns(new Categoria { CategoriaId = 3, Nome = "Mercado", Tipo = Tipo.Saida });

        _service = new TransacaoService(_transacoes, _contas, _categorias, new FakeTimeProvider(Agora));
    }

    [Fact]
    public async Task Criar_SemData_UsaDataAtual()
    {
        Transacao? salva = null;
        await _transacoes.AdicionarTransacaoAsync(Arg.Do<Transacao>(t => salva = t), Arg.Any<CancellationToken>());

        await _service.CriarTransacaoAsync(NovaTransacao());

        Assert.NotNull(salva);
        Assert.Equal(new DateOnly(2026, 10, 8), salva.Data);
        Assert.Equal(Agora.UtcDateTime, salva.DataCriacao);
        await _transacoes.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Criar_ComData_RespeitaDataInformada()
    {
        Transacao? salva = null;
        await _transacoes.AdicionarTransacaoAsync(Arg.Do<Transacao>(t => salva = t), Arg.Any<CancellationToken>());
        var dto = NovaTransacao();
        dto.Data = new DateOnly(2026, 1, 15);

        await _service.CriarTransacaoAsync(dto);

        Assert.Equal(new DateOnly(2026, 1, 15), salva!.Data);
    }

    [Fact]
    public async Task Criar_ContaInexistente_LancaContaNaoEncontrada()
    {
        var dto = NovaTransacao();
        dto.ContaId = 99;

        await Assert.ThrowsAsync<ContaNaoEncontradaException>(() => _service.CriarTransacaoAsync(dto));
        await _transacoes.DidNotReceive().SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Criar_CategoriaInexistente_LancaCategoriaNaoEncontrada()
    {
        var dto = NovaTransacao();
        dto.CategoriaId = 99;

        await Assert.ThrowsAsync<CategoriaNaoEncontradaException>(() => _service.CriarTransacaoAsync(dto));
    }

    [Fact]
    public async Task Criar_CategoriaDeOutroTipo_LancaValidacao()
    {
        var dto = NovaTransacao();
        dto.CategoriaId = 3; // "Mercado" é de saída; a transação é de entrada.

        var ex = await Assert.ThrowsAsync<ValidacaoNegocioException>(() => _service.CriarTransacaoAsync(dto));

        Assert.Contains("Mercado", ex.Message);
        await _transacoes.DidNotReceive().SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Buscar_Inexistente_LancaTransacaoNaoEncontrada()
    {
        var ex = await Assert.ThrowsAsync<TransacaoNaoEncontradaException>(() => _service.BuscarTransacaoPorIdAsync(5));
        Assert.Equal("Transação 5 não encontrada.", ex.Message);
    }

    [Fact]
    public async Task Atualizar_SemData_MantemDataOriginal()
    {
        var existente = TransacaoExistente();
        _transacoes.BuscarTransacaoParaEdicaoAsync(7, Arg.Any<CancellationToken>()).Returns(existente);

        await _service.AtualizarTransacaoAsync(7, new TransacaoAtualizarDto
        {
            Descricao = "  Nova  ",
            Valor = 10,
            Tipo = Tipo.Entrada,
            ContaId = 1,
            CategoriaId = 2
        });

        Assert.Equal("Nova", existente.Descricao);
        Assert.Equal(new DateOnly(2026, 3, 1), existente.Data);
        await _transacoes.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Atualizar_TrocandoSoOTipo_ValidaCategoriaAtual()
    {
        _transacoes.BuscarTransacaoParaEdicaoAsync(7, Arg.Any<CancellationToken>()).Returns(TransacaoExistente());

        await Assert.ThrowsAsync<ValidacaoNegocioException>(() => _service.AtualizarTransacaoAsync(7, new TransacaoAtualizarDto
        {
            Descricao = "Salário",
            Valor = 10,
            Tipo = Tipo.Saida,
            ContaId = 1,
            CategoriaId = 2
        }));
    }

    [Fact]
    public async Task Listar_RetornaResultadoPaginado()
    {
        var filtro = new TransacaoFiltroDto { Pagina = 2, TamanhoPagina = 1 };
        var item = TransacaoExistente();
        item.Conta = new Conta { ContaId = 1, Nome = "Corrente" };
        item.Categoria = new Categoria { CategoriaId = 2, Nome = "Salário" };
        _transacoes.ListarTransacoesAsync(filtro, Arg.Any<CancellationToken>()).Returns((new List<Transacao> { item }, 3));

        var resultado = await _service.ListarTransacoesAsync(filtro);

        Assert.Single(resultado.Itens);
        Assert.Equal(3, resultado.Total);
        Assert.Equal(3, resultado.TotalPaginas);
        Assert.Equal("Corrente", resultado.Itens[0].ContaNome);
    }

    [Fact]
    public async Task Listar_PeriodoInvertido_LancaValidacao()
    {
        var filtro = new TransacaoFiltroDto { DataInicio = new DateOnly(2026, 10, 31), DataFim = new DateOnly(2026, 10, 1) };

        await Assert.ThrowsAsync<ValidacaoNegocioException>(() => _service.ListarTransacoesAsync(filtro));
    }

    private static TransacaoCriarDto NovaTransacao() => new()
    {
        Descricao = "Salário",
        Valor = 100,
        Tipo = Tipo.Entrada,
        ContaId = 1,
        CategoriaId = 2
    };

    private static Transacao TransacaoExistente() => new()
    {
        TransacaoId = 7,
        Descricao = "Antiga",
        Data = new DateOnly(2026, 3, 1),
        Tipo = Tipo.Entrada,
        ContaId = 1,
        CategoriaId = 2
    };
}
