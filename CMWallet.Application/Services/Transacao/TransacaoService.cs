using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

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

        public async Task<int> CriarTransacaoAsync(TransacaoCriarDto dto)
        {
            await GarantirContaECategoriaExistemAsync(dto.ContaId, dto.CategoriaId);

            var agora = _timeProvider.GetUtcNow().UtcDateTime;

            var transacao = new Transacao
            {
                Descricao = dto.Descricao.Trim(),
                Valor = dto.Valor,
                Data = dto.Data ?? agora,
                Tipo = dto.Tipo,
                CategoriaId = dto.CategoriaId,
                ContaId = dto.ContaId,
                Pago = dto.Pago,
                DataCriacao = agora
            };

            await _transacaoRepository.AdicionarTransacaoAsync(transacao);
            await _transacaoRepository.SalvarAlteracoesAsync();

            return transacao.TransacaoId;
        }

        public async Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId)
        {
            var transacao = await ObterTransacaoAsync(transacaoId);

            return ParaDto(transacao);
        }

        public async Task<PagedResult<TransacaoResponseDto>> ListarTransacoesAsync(TransacaoFiltroDto filtro)
        {
            var (itens, total) = await _transacaoRepository.ListarTransacoesAsync(filtro);

            return new PagedResult<TransacaoResponseDto>
            {
                Itens = itens.Select(ParaDto).ToList(),
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                Total = total
            };
        }

        public async Task ApagarTransacaoAsync(int transacaoId)
        {
            var transacao = await ObterTransacaoAsync(transacaoId);

            _transacaoRepository.DeletarTransacao(transacao);
            await _transacaoRepository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarTransacaoAsync(int transacaoId, TransacaoAtualizarDto dto)
        {
            var transacao = await ObterTransacaoAsync(transacaoId);

            if (transacao.ContaId != dto.ContaId || transacao.CategoriaId != dto.CategoriaId)
            {
                await GarantirContaECategoriaExistemAsync(dto.ContaId, dto.CategoriaId);
            }

            transacao.Descricao = dto.Descricao.Trim();
            transacao.Valor = dto.Valor;
            transacao.Data = dto.Data ?? transacao.Data;
            transacao.Tipo = dto.Tipo;
            transacao.CategoriaId = dto.CategoriaId;
            transacao.ContaId = dto.ContaId;
            transacao.Pago = dto.Pago;

            await _transacaoRepository.SalvarAlteracoesAsync();
        }

        private async Task<Transacao> ObterTransacaoAsync(int transacaoId)
        {
            return await _transacaoRepository.BuscarTransacaoPorIdAsync(transacaoId)
                ?? throw new TransacaoNaoEncontradaException(transacaoId);
        }

        private async Task GarantirContaECategoriaExistemAsync(int contaId, int categoriaId)
        {
            if (await _contaRepository.BuscarContaPorIdAsync(contaId) is null)
            {
                throw new ContaNaoEncontradaException(contaId);
            }

            if (await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId) is null)
            {
                throw new CategoriaNaoEncontradaException(categoriaId);
            }
        }

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
