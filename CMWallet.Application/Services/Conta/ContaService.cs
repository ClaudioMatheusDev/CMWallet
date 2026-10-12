using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Services
{
    public class ContaService : IContaService
    {
        private readonly IContaRepository _contaRepository;
        private readonly TimeProvider _timeProvider;

        public ContaService(IContaRepository contaRepository, TimeProvider timeProvider)
        {
            _contaRepository = contaRepository;
            _timeProvider = timeProvider;
        }

        public async Task<int> CriarContaAsync(ContaCriarDto dto, CancellationToken cancellationToken = default)
        {
            var agora = _timeProvider.GetUtcNow().UtcDateTime;
            var conta = new Conta
            {
                Nome = dto.Nome.Trim(),
                SaldoInicial = dto.SaldoInicial,
                TipoConta = dto.TipoConta,
                DataCriacao = agora,
                DataAtualizacao = agora
            };

            await _contaRepository.AdicionarContaAsync(conta, cancellationToken);
            await _contaRepository.SalvarAlteracoesAsync(cancellationToken);

            return conta.ContaId;
        }

        public async Task<ContaResponseDto> BuscarContaPorIdAsync(int contaId, CancellationToken cancellationToken = default)
        {
            var conta = await _contaRepository.BuscarContaComSaldoAsync(contaId, cancellationToken)
                ?? throw new ContaNaoEncontradaException(contaId);

            return ParaDto(conta);
        }

        public async Task<List<ContaResponseDto>> ListarContasAsync(CancellationToken cancellationToken = default)
        {
            var contas = await _contaRepository.ListarContasComSaldoAsync(cancellationToken);
            return contas.Select(ParaDto).ToList();
        }

        public async Task ApagarContaAsync(int contaId, CancellationToken cancellationToken = default)
        {
            var conta = await ObterParaEdicaoAsync(contaId, cancellationToken);

            var (transacoes, metas) = await _contaRepository.ContarVinculosAsync(contaId, cancellationToken);
            if (transacoes > 0 || metas > 0)
            {
                var vinculos = new List<string>();
                if (transacoes > 0) vinculos.Add(Texto.Quantidade(transacoes, "transação", "transações"));
                if (metas > 0) vinculos.Add(Texto.Quantidade(metas, "meta", "metas"));

                throw new ConflitoNegocioException(
                    $"A conta \"{conta.Nome}\" possui {string.Join(" e ", vinculos)}. " +
                    "Exclua esses registros ou mova-os para outra conta antes de excluí-la.");
            }

            _contaRepository.DeletarConta(conta);
            await _contaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        public async Task AtualizarContaAsync(int contaId, ContaAtualizarDto dto, CancellationToken cancellationToken = default)
        {
            var conta = await ObterParaEdicaoAsync(contaId, cancellationToken);

            conta.Nome = dto.Nome.Trim();
            conta.SaldoInicial = dto.SaldoInicial;
            conta.TipoConta = dto.TipoConta;
            conta.DataAtualizacao = _timeProvider.GetUtcNow().UtcDateTime;

            await _contaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        private async Task<Conta> ObterParaEdicaoAsync(int contaId, CancellationToken cancellationToken)
        {
            return await _contaRepository.BuscarContaParaEdicaoAsync(contaId, cancellationToken)
                ?? throw new ContaNaoEncontradaException(contaId);
        }

        private static ContaResponseDto ParaDto(ContaComSaldo item) => new()
        {
            ContaId = item.Conta.ContaId,
            Nome = item.Conta.Nome,
            SaldoInicial = item.Conta.SaldoInicial,
            SaldoAtual = item.SaldoAtual,
            TipoConta = item.Conta.TipoConta,
            DataCriacao = item.Conta.DataCriacao,
            DataAtualizacao = item.Conta.DataAtualizacao
        };
    }
}
