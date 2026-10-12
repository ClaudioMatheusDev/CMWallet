using System.Linq.Expressions;
using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using CMWallet.Domain.Enums;
using CMWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.Infrastructure.Repositories
{
    public class ContaRepository : IContaRepository
    {
        /// <summary>Saldo inicial + entradas pagas − saídas pagas, calculado numa subconsulta no banco.</summary>
        private static readonly Expression<Func<Conta, ContaComSaldo>> ComSaldo = c => new ContaComSaldo(
            c,
            c.SaldoInicial + (c.Transacoes
                .Where(t => t.Pago)
                .Sum(t => (decimal?)(t.Tipo == Tipo.Entrada ? t.Valor : -t.Valor)) ?? 0m));

        private readonly ApplicationDbContext _context;

        public ContaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarContaAsync(Conta conta, CancellationToken cancellationToken = default)
        {
            await _context.Contas.AddAsync(conta, cancellationToken);
        }

        public async Task<Conta?> BuscarContaParaEdicaoAsync(int contaId, CancellationToken cancellationToken = default)
        {
            return await _context.Contas.FirstOrDefaultAsync(c => c.ContaId == contaId, cancellationToken);
        }

        public async Task<ContaComSaldo?> BuscarContaComSaldoAsync(int contaId, CancellationToken cancellationToken = default)
        {
            return await _context.Contas
                .AsNoTracking()
                .Where(c => c.ContaId == contaId)
                .Select(ComSaldo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ContaComSaldo>> ListarContasComSaldoAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Contas
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .Select(ComSaldo)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExisteContaAsync(int contaId, CancellationToken cancellationToken = default)
        {
            return await _context.Contas.AnyAsync(c => c.ContaId == contaId, cancellationToken);
        }

        public async Task<(int Transacoes, int Metas)> ContarVinculosAsync(int contaId, CancellationToken cancellationToken = default)
        {
            var transacoes = await _context.Transacoes.CountAsync(t => t.ContaId == contaId, cancellationToken);
            var metas = await _context.MetasFinanceiras.CountAsync(m => m.ContaId == contaId, cancellationToken);
            return (transacoes, metas);
        }

        public void DeletarConta(Conta conta)
        {
            _context.Contas.Remove(conta);
        }

        public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
