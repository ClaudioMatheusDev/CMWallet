using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Services
{
    public class ContaService : IContaService
    {
        private readonly IContaRepository _contaRepository;

        public ContaService(IContaRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<int> CriarContaAsync(ContaCriarDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new ValidacaoNegocioException("O nome da conta é obrigatório.");
            }

            var conta = new Conta
            {
                Nome = dto.Nome,
                SaldoInicial = dto.SaldoInicial,
                TipoConta = dto.TipoConta,
                DataCriacao = DateTime.UtcNow.AddHours(-3),
                DataAtualizacao = DateTime.UtcNow.AddHours(-3)
            };

            await _contaRepository.AdicionarContaAsync(conta);
            await _contaRepository.SalvarAlteracoesAsync();

            return conta.ContaId;
        }

        public async Task<ContaReponseDto> BuscarContaPorIdAsync(int contaId)
        {
            var conta = await _contaRepository.BuscarContasPorIdAsync(contaId);

            if (conta == null)
            {
                throw new ContaNaoEncontradaException(contaId);
            }

            return new ContaReponseDto
            {
                ContaId = conta.ContaId,
                Nome = conta.Nome,
                SaldoInicial = conta.SaldoInicial,
                TipoConta = conta.TipoConta,
                DataCriacao = conta.DataCriacao,
                DataAtualizacao = conta.DataAtualizacao
            };
        }

        public async Task<List<ContaReponseDto>> ListarContasAsync()
        {
            var contas = await _contaRepository.ListarTodasContas();

            return contas.Select(conta => new ContaReponseDto
            {
                ContaId = conta.ContaId,
                Nome = conta.Nome,
                SaldoInicial = conta.SaldoInicial,
                TipoConta = conta.TipoConta,
                DataCriacao = conta.DataCriacao,
                DataAtualizacao = conta.DataAtualizacao
            }).ToList();
        }

        public async Task<bool> ApagarContaAsync(int ContaId)
        {
            var conta = await _contaRepository.BuscarContasPorIdAsync(ContaId);

            if (conta == null)
            {
                throw new ContaNaoEncontradaException(ContaId);
            }

            _contaRepository.DeletarConta(conta);
            await _contaRepository.SalvarAlteracoesAsync();

            return true;
        }

        public async Task<bool> AtualizarContaAsync(int ContaId, ContaAtualizarDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new ValidacaoNegocioException("O nome da conta é obrigatório.");
            }

            var conta = await _contaRepository.BuscarContasPorIdAsync(ContaId);

            if (conta == null)
            {
                throw new ContaNaoEncontradaException(ContaId);
            }

            conta.Nome = dto.Nome;
            conta.SaldoInicial = dto.SaldoInicial;
            conta.TipoConta = dto.TipoConta;
            conta.DataAtualizacao = DateTime.UtcNow.AddHours(-3);

            _contaRepository.AtualizarConta(conta);
            await _contaRepository.SalvarAlteracoesAsync();

            return true;
        }
    }
}
