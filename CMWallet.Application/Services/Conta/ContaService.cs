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

        public async Task<int> CriarContaAsync(ContaCriarDto dto)
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

            await _contaRepository.AdicionarContaAsync(conta);
            await _contaRepository.SalvarAlteracoesAsync();

            return conta.ContaId;
        }

        public async Task<ContaResponseDto> BuscarContaPorIdAsync(int contaId)
        {
            var conta = await ObterContaAsync(contaId);

            return ParaDto(conta);
        }

        public async Task<List<ContaResponseDto>> ListarContasAsync()
        {
            var contas = await _contaRepository.ListarTodasContas();

            return contas.Select(ParaDto).ToList();
        }

        public async Task ApagarContaAsync(int contaId)
        {
            var conta = await ObterContaAsync(contaId);

            _contaRepository.DeletarConta(conta);
            await _contaRepository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarContaAsync(int contaId, ContaAtualizarDto dto)
        {
            var conta = await ObterContaAsync(contaId);

            conta.Nome = dto.Nome.Trim();
            conta.SaldoInicial = dto.SaldoInicial;
            conta.TipoConta = dto.TipoConta;
            conta.DataAtualizacao = _timeProvider.GetUtcNow().UtcDateTime;

            await _contaRepository.SalvarAlteracoesAsync();
        }

        private async Task<Conta> ObterContaAsync(int contaId)
        {
            return await _contaRepository.BuscarContaPorIdAsync(contaId)
                ?? throw new ContaNaoEncontradaException(contaId);
        }

        private static ContaResponseDto ParaDto(Conta conta) => new()
        {
            ContaId = conta.ContaId,
            Nome = conta.Nome,
            SaldoInicial = conta.SaldoInicial,
            TipoConta = conta.TipoConta,
            DataCriacao = conta.DataCriacao,
            DataAtualizacao = conta.DataAtualizacao
        };
    }
}
