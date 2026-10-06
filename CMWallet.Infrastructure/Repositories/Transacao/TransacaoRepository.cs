using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using CMWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.Infrastructure.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {

        private readonly ApplicationDbContext _context;

        public TransacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarTransacaoAsync(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao);
        }

        public async Task<Transacao?> BuscarTransacoesPorIdAsync(int TransacaoId)
        {
            return await _context.Transacoes.FirstOrDefaultAsync(t => t.TransacaoId == TransacaoId);
        }
        public async Task<List<Transacao>> ListarTodasTransacoes()
        {
            return await _context.Transacoes.ToListAsync();
        }

        public void DeletarTransacao(Transacao transacao)
        {
            _context.Transacoes.Remove(transacao);
        }

        public void AtualizarTransacao(Transacao transacao)
        {
            _context.Transacoes.Update(transacao);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
        }
    }
}
