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

        public async Task AdicionarTransacaoAsync(Transacao transacao, CancellationToken cancellationToken = default)
        {
            await _context.Transacoes.AddAsync(transacao, cancellationToken);
        }

        public async Task<Transacao?> BuscarTransacaoParaEdicaoAsync(int transacaoId, CancellationToken cancellationToken = default)
        {
            return await _context.Transacoes.FirstOrDefaultAsync(t => t.TransacaoId == transacaoId, cancellationToken);
        }

        public async Task<Transacao?> BuscarTransacaoPorIdAsync(int transacaoId, CancellationToken cancellationToken = default)
        {
            return await _context.Transacoes
                .AsNoTracking()
                .Include(t => t.Categoria)
                .Include(t => t.Conta)
                .FirstOrDefaultAsync(t => t.TransacaoId == transacaoId, cancellationToken);
        }

        public async Task<(List<Transacao> Itens, int Total)> ListarTransacoesAsync(
            TransacaoFiltroDto filtro,
            CancellationToken cancellationToken = default)
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

            var total = await query.CountAsync(cancellationToken);

            var itens = await query
                .Include(t => t.Categoria)
                .Include(t => t.Conta)
                .OrderByDescending(t => t.Data)
                .ThenByDescending(t => t.TransacaoId)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync(cancellationToken);

            return (itens, total);
        }

        public async Task<List<TotalPorCategoria>> AgruparPorCategoriaAsync(
            DateOnly dataInicio,
            DateOnly dataFim,
            int? contaId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Transacoes.Where(t => t.Data >= dataInicio && t.Data <= dataFim);

            if (contaId.HasValue)
            {
                query = query.Where(t => t.ContaId == contaId.Value);
            }

            return await query
                .GroupBy(t => new { t.CategoriaId, t.Categoria.Nome, t.Tipo, t.Pago })
                .Select(g => new TotalPorCategoria(
                    g.Key.CategoriaId,
                    g.Key.Nome,
                    g.Key.Tipo,
                    g.Key.Pago,
                    g.Sum(t => t.Valor),
                    g.Count()))
                .ToListAsync(cancellationToken);
        }

        public void DeletarTransacao(Transacao transacao)
        {
            _context.Transacoes.Remove(transacao);
        }

        public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
