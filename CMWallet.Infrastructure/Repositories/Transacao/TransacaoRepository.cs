using CMWallet.Application.Dtos;
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

        public async Task<Transacao?> BuscarTransacaoPorIdAsync(int transacaoId)
        {
            return await _context.Transacoes
                .Include(t => t.Categoria)
                .Include(t => t.Conta)
                .FirstOrDefaultAsync(t => t.TransacaoId == transacaoId);
        }

        public async Task<(List<Transacao> Itens, int Total)> ListarTransacoesAsync(TransacaoFiltroDto filtro)
        {
            var query = _context.Transacoes.AsNoTracking();

            if (filtro.ContaId.HasValue)
            {
                query = query.Where(t => t.ContaId == filtro.ContaId.Value);
            }

            if (filtro.CategoriaId.HasValue)
            {
                query = query.Where(t => t.CategoriaId == filtro.CategoriaId.Value);
            }

            if (filtro.Tipo.HasValue)
            {
                query = query.Where(t => t.Tipo == filtro.Tipo.Value);
            }

            if (filtro.DataInicio.HasValue)
            {
                query = query.Where(t => t.Data >= filtro.DataInicio.Value);
            }

            if (filtro.DataFim.HasValue)
            {
                query = query.Where(t => t.Data <= filtro.DataFim.Value);
            }

            var total = await query.CountAsync();

            var itens = await query
                .Include(t => t.Categoria)
                .Include(t => t.Conta)
                .OrderByDescending(t => t.Data)
                .ThenByDescending(t => t.TransacaoId)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();

            return (itens, total);
        }

        public void DeletarTransacao(Transacao transacao)
        {
            _context.Transacoes.Remove(transacao);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
