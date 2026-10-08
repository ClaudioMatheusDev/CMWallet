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
        _contas.BuscarContaPorIdAsync(1).Returns(new Conta { ContaId = 1, Nome = "Corrente" });
        _categorias.BuscarCategoriaPorIdAsync(2).Returns(new Categoria { CategoriaId = 2, Nome = "Salário" });

        _service = new TransacaoService(_transacoes, _contas, _categorias, new FakeTimeProvider(Agora));
    }

    [Fact]
    public async Task Criar_SemData_UsaMomentoAtualEmUtc()
    {
        Transacao? salva = null;
        await _transacoes.AdicionarTransacaoAsync(Arg.Do<Transacao>(t => salva = t));

        await _service.CriarTransacaoAsync(NovaTransacao());

        Assert.NotNull(salva);
        Assert.Equal(Agora.UtcDateTime, salva.Data);
        Assert.Equal(Agora.UtcDateTime, salva.DataCriacao);
        await _transacoes.Received(1).SalvarAlteracoesAsync();
    }

    [Fact]
    public async Task Criar_ComData_RespeitaDataInformada()
    {
        var data = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        Transacao? salva = null;
        await _transacoes.AdicionarTransacaoAsync(Arg.Do<Transacao>(t => salva = t));

        var dto = NovaTransacao();
        dto.Data = data;
        await _service.CriarTransacaoAsync(dto);

        Assert.Equal(data, salva!.Data);
    }

    [Fact]
    public async Task Criar_ContaInexistente_LancaContaNaoEncontrada()
    {
        var dto = NovaTransacao();
        dto.ContaId = 99;

        await Assert.ThrowsAsync<ContaNaoEncontradaException>(() => _service.CriarTransacaoAsync(dto));
        await _transacoes.DidNotReceive().SalvarAlteracoesAsync();
    }

    [Fact]
    public async Task Criar_CategoriaInexistente_LancaCategoriaNaoEncontrada()
    {
        var dto = NovaTransacao();
        dto.CategoriaId = 99;

        await Assert.ThrowsAsync<CategoriaNaoEncontradaException>(() => _service.CriarTransacaoAsync(dto));
    }

    [Fact]
    public async Task Buscar_Inexistente_LancaTransacaoNaoEncontrada()
    {
        await Assert.ThrowsAsync<TransacaoNaoEncontradaException>(() => _service.BuscarTransacaoPorIdAsync(5));
    }

    [Fact]
    public async Task Atualizar_SemData_MantemDataOriginal()
    {
        var original = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        var existente = new Transacao
        {
            TransacaoId = 7,
            Descricao = "Antiga",
            Data = original,
            ContaId = 1,
            CategoriaId = 2,
            Conta = new Conta { ContaId = 1, Nome = "Corrente" },
            Categoria = new Categoria { CategoriaId = 2, Nome = "Salário" }
        };
        _transacoes.BuscarTransacaoPorIdAsync(7).Returns(existente);

        await _service.AtualizarTransacaoAsync(7, new TransacaoAtualizarDto
        {
            Descricao = "  Nova  ",
            Valor = 10,
            Tipo = Tipo.Saida,
            ContaId = 1,
            CategoriaId = 2
        });

        Assert.Equal("Nova", existente.Descricao);
        Assert.Equal(original, existente.Data);
        await _transacoes.Received(1).SalvarAlteracoesAsync();
    }

    [Fact]
    public async Task Listar_RetornaResultadoPaginado()
    {
        var filtro = new TransacaoFiltroDto { Pagina = 2, TamanhoPagina = 1 };
        var item = new Transacao
        {
            TransacaoId = 1,
            Descricao = "x",
            Conta = new Conta { ContaId = 1, Nome = "Corrente" },
            Categoria = new Categoria { CategoriaId = 2, Nome = "Salário" }
        };
        _transacoes.ListarTransacoesAsync(filtro).Returns((new List<Transacao> { item }, 3));

        var resultado = await _service.ListarTransacoesAsync(filtro);

        Assert.Single(resultado.Itens);
        Assert.Equal(3, resultado.Total);
        Assert.Equal(3, resultado.TotalPaginas);
        Assert.Equal("Corrente", resultado.Itens[0].ContaNome);
    }

    private static TransacaoCriarDto NovaTransacao() => new()
    {
        Descricao = "Salário",
        Valor = 100,
        Tipo = Tipo.Entrada,
        ContaId = 1,
        CategoriaId = 2
    };
}
