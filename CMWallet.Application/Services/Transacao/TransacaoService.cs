using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _service;

        public TransacaoService(ITransacaoRepository service)
        {
            _service = service;
        }

        public async Task<int> CriarTransacaoAsync(TransacaoCriarDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
            {
                throw new ValidacaoNegocioException("A descrição da transação é obrigatória.");
            }

            if (dto.Valor <= 0)
            {
                throw new ValidacaoNegocioException("O valor da transação deve ser maior que zero.");
            }

            var transacao = new Transacao
            {
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                Data = DateTime.UtcNow.AddHours(-3),
                Tipo = dto.Tipo,
                CategoriaId = dto.CategoriaId,
                ContaId = dto.ContaId,
                Pago = dto.Pago,
                DataCriacao = DateTime.UtcNow.AddHours(-3)
            };

            await _service.AdicionarTransacaoAsync(transacao);
            await _service.SalvarAlteracoesAsync();

            return transacao.TransacaoId;
        }

        public async Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId)
        {
            var transacao = await _service.BuscarTransacoesPorIdAsync(transacaoId);

            if (transacao == null)
            {
                throw new TransacaoNaoEncontradaException(transacaoId);
            }

            return new TransacaoResponseDto
            {
                TransacaoId = transacao.TransacaoId,
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                Data = transacao.Data,
                Tipo = transacao.Tipo,
                CategoriaId = transacao.CategoriaId,
                Categoria = transacao.Categoria,
                ContaId = transacao.ContaId,
                Conta = transacao.Conta,
                Pago = transacao.Pago
            };
        }

        public async Task<List<TransacaoResponseDto>> ListarTransacoesAsync()
        {
            var transacoes = await _service.ListarTodasTransacoes();

            return transacoes.Select(transacao => new TransacaoResponseDto
            {
                TransacaoId = transacao.TransacaoId,
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                Data = transacao.Data,
                Tipo = transacao.Tipo,
                CategoriaId = transacao.CategoriaId,
                Categoria = transacao.Categoria,
                ContaId = transacao.ContaId,
                Conta = transacao.Conta,
                Pago = transacao.Pago
            }).ToList();
        }

        public async Task ApagarTransacaoAsync(int transacaoId)
        {
            var transacao = await _service.BuscarTransacoesPorIdAsync(transacaoId);

            if (transacao == null)
            {
                throw new TransacaoNaoEncontradaException(transacaoId);
            }

            _service.DeletarTransacao(transacao);
            await _service.SalvarAlteracoesAsync();
        }

        public async Task AtualizarTransacaoAsync(int transacaoId, TransacaoAtualizarDto dto)
        {
            var transacao = await _service.BuscarTransacoesPorIdAsync(transacaoId);

            if (transacao == null)
            {
                throw new TransacaoNaoEncontradaException(transacaoId);
            }

            if (string.IsNullOrWhiteSpace(dto.Descricao))
            {
                throw new ValidacaoNegocioException("A descrição da transação é obrigatória.");
            }

            if (dto.Valor <= 0)
            {
                throw new ValidacaoNegocioException("O valor da transação deve ser maior que zero.");
            }

            transacao.Descricao = dto.Descricao;
            transacao.Valor = dto.Valor;
            transacao.Tipo = dto.Tipo;
            transacao.CategoriaId = dto.CategoriaId;
            transacao.ContaId = dto.ContaId;
            transacao.Pago = dto.Pago;

            _service.AtualizarTransacao(transacao);
            await _service.SalvarAlteracoesAsync();
        }
    }
}
