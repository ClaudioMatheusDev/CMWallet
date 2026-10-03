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

        public void AtualizarConta(Conta conta)
        {
           _context.Contas.Update(conta);
        }

        public async Task<Conta?> BuscarContasPorIdAsync(int ContaId)
        {
          return await _context.Contas.FirstOrDefaultAsync(c => c.ContaId == ContaId);
        }

        public void DeletarConta(Conta conta)
        {
            _context.Contas.Remove(conta);
        }

        public async Task<List<Conta>> ListarTodasContas()
        {
           return await _context.Contas.ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
