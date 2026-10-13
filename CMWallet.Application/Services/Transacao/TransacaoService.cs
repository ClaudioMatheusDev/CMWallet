using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using CMWallet.Domain.Enums;

namespace CMWallet.Application.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IContaRepository _contaRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly TimeProvider _timeProvider;

        public TransacaoService(
            ITransacaoRepository transacaoRepository,
            IContaRepository contaRepository,
            ICategoriaRepository categoriaRepository,
            TimeProvider timeProvider)
        {
            _transacaoRepository = transacaoRepository;
            _contaRepository = contaRepository;
            _categoriaRepository = categoriaRepository;
            _timeProvider = timeProvider;
        }

        public async Task<int> CriarTransacaoAsync(TransacaoCriarDto dto, CancellationToken cancellationToken = default)
        {
            await GarantirContaExisteAsync(dto.ContaId, cancellationToken);
            await GarantirCategoriaCompativelAsync(dto.CategoriaId, dto.Tipo, cancellationToken);

            var agora = _timeProvider.GetUtcNow();
            var transacao = new Transacao
            {
                Descricao = dto.Descricao.Trim(),
                Valor = dto.Valor,
                Data = dto.Data ?? DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime),
                Tipo = dto.Tipo,
                CategoriaId = dto.CategoriaId,
                ContaId = dto.ContaId,
                Pago = dto.Pago,
                DataCriacao = agora.UtcDateTime
            };

            await _transacaoRepository.AdicionarTransacaoAsync(transacao, cancellationToken);
            await _transacaoRepository.SalvarAlteracoesAsync(cancellationToken);

            return transacao.TransacaoId;
        }

        public async Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId, CancellationToken cancellationToken = default)
        {
            var transacao = await _transacaoRepository.BuscarTransacaoPorIdAsync(transacaoId, cancellationToken)
                ?? throw new TransacaoNaoEncontradaException(transacaoId);

            return ParaDto(transacao);
        }

        public async Task<PagedResult<TransacaoResponseDto>> ListarTransacoesAsync(
            TransacaoFiltroDto filtro,
            CancellationToken cancellationToken = default)
        {
            if (filtro.DataInicio > filtro.DataFim)
            {
                throw new ValidacaoNegocioException("A data final deve ser igual ou posterior à data inicial.");
            }

            var (itens, total) = await _transacaoRepository.ListarTransacoesAsync(filtro, cancellationToken);

            return new PagedResult<TransacaoResponseDto>
            {
                Itens = itens.Select(ParaDto).ToList(),
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                Total = total
            };
        }

        public async Task ApagarTransacaoAsync(int transacaoId, CancellationToken cancellationToken = default)
        {
            var transacao = await ObterParaEdicaoAsync(transacaoId, cancellationToken);

            _transacaoRepository.DeletarTransacao(transacao);
            await _transacaoRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        public async Task AtualizarTransacaoAsync(
            int transacaoId,
            TransacaoAtualizarDto dto,
            CancellationToken cancellationToken = default)
        {
            var transacao = await ObterParaEdicaoAsync(transacaoId, cancellationToken);

            if (transacao.ContaId != dto.ContaId)
            {
                await GarantirContaExisteAsync(dto.ContaId, cancellationToken);
            }
            if (transacao.CategoriaId != dto.CategoriaId || transacao.Tipo != dto.Tipo)
            {
                await GarantirCategoriaCompativelAsync(dto.CategoriaId, dto.Tipo, cancellationToken);
            }

            transacao.Descricao = dto.Descricao.Trim();
            transacao.Valor = dto.Valor;
            transacao.Data = dto.Data ?? transacao.Data;
            transacao.Tipo = dto.Tipo;
            transacao.CategoriaId = dto.CategoriaId;
            transacao.ContaId = dto.ContaId;
            transacao.Pago = dto.Pago;

            await _transacaoRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        private async Task<Transacao> ObterParaEdicaoAsync(int transacaoId, CancellationToken cancellationToken)
        {
            return await _transacaoRepository.BuscarTransacaoParaEdicaoAsync(transacaoId, cancellationToken)
                ?? throw new TransacaoNaoEncontradaException(transacaoId);
        }

        private async Task GarantirContaExisteAsync(int contaId, CancellationToken cancellationToken)
        {
            if (!await _contaRepository.ExisteContaAsync(contaId, cancellationToken))
            {
                throw new ContaNaoEncontradaException(contaId);
            }
        }

        /// <summary>Uma entrada só pode usar categoria de entrada, e uma saída, categoria de saída.</summary>
        private async Task GarantirCategoriaCompativelAsync(int categoriaId, Tipo tipo, CancellationToken cancellationToken)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId, cancellationToken)
                ?? throw new CategoriaNaoEncontradaException(categoriaId);

            if (categoria.Tipo != tipo)
            {
                throw new ValidacaoNegocioException(
                    $"A categoria \"{categoria.Nome}\" é de {DescreverTipo(categoria.Tipo)} " +
                    $"e não pode ser usada em uma transação de {DescreverTipo(tipo)}.");
            }
        }

        private static string DescreverTipo(Tipo tipo) => tipo == Tipo.Entrada ? "entrada" : "saída";

        private static TransacaoResponseDto ParaDto(Transacao transacao) => new()
        {
            TransacaoId = transacao.TransacaoId,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Data = transacao.Data,
            Tipo = transacao.Tipo,
            CategoriaId = transacao.CategoriaId,
            CategoriaNome = transacao.Categoria.Nome,
            ContaId = transacao.ContaId,
            ContaNome = transacao.Conta.Nome,
            Pago = transacao.Pago
        };
    }
}
