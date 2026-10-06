using CMWallet.Application.Dtos;
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
            var transacao = new Transacao
            {
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                Data = DateTime.UtcNow.AddHours(-3),
                Tipo = dto.Tipo,
                CategoriaId = dto.CategoriaId,
                ContaId = dto.ContaId,
                Pago = dto.Pago
            };

            await _service.AdicionarTransacaoAsync(transacao);
            await _service.SalvarAlteracoesAsync();

            return transacao.TransacaoId;
        }


        public async Task<TransacaoResponseDto> BuscarTransacaoPorIdAsync(int transacaoId)
        {
            var transacoes = await _service.BuscarTransacoesPorIdAsync(transacaoId);

            if (transacoes == null)
            {
                throw new Exception("Transação não encontrada.");
            }

            return new TransacaoResponseDto
            {
                TransacaoId = transacoes.TransacaoId,
                Descricao = transacoes.Descricao,
                Valor = transacoes.Valor,
                Data = transacoes.Data,
                Tipo = transacoes.Tipo,
                CategoriaId = transacoes.CategoriaId,
                Categoria = transacoes.Categoria,
                ContaId = transacoes.ContaId,
                Conta = transacoes.Conta,
                Pago = transacoes.Pago
            };
        }


        public async Task<List<TransacaoResponseDto>> ListarTransacoesAsync()
        {
            var transacoes = await _service.ListarTodasTransacoes();

            return transacoes.Select(transacoes => new TransacaoResponseDto
            {
                TransacaoId = transacoes.TransacaoId,
                Descricao = transacoes.Descricao,
                Valor = transacoes.Valor,
                Data = transacoes.Data,
                Tipo = transacoes.Tipo,
                CategoriaId = transacoes.CategoriaId,
                Categoria = transacoes.Categoria,
                ContaId = transacoes.ContaId,
                Conta = transacoes.Conta,
                Pago = transacoes.Pago
            }).ToList();
        }

        public async Task ApagarTransacaoAsync(int TransacaoId)
        {
          var transacao = _service.BuscarTransacoesPorIdAsync(TransacaoId).Result;
            if (transacao == null)
            {
                throw new Exception("Transação não encontrada.");
            }
            _service.DeletarTransacao(transacao);
            await _service.SalvarAlteracoesAsync();
        }

        public async Task AtualizarTransacaoAsync(int TransacaoId, TransacaoAtualizarDto dto)
        {
            var transacao = await _service.BuscarTransacoesPorIdAsync(TransacaoId);

            if (transacao == null)
            {
                throw new Exception("Transação não encontrada.");
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
