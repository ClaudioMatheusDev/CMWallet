using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using CMWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.Infrastructure.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly ApplicationDbContext _context;

        public ContaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarContaAsync(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
        }

        public async Task<Conta?> BuscarContaPorIdAsync(int contaId)
        {
            return await _context.Contas.FirstOrDefaultAsync(c => c.ContaId == contaId);
        }

        public void DeletarConta(Conta conta)
        {
            _context.Contas.Remove(conta);
        }

        public async Task<List<Conta>> ListarTodasContas()
        {
            return await _context.Contas.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
