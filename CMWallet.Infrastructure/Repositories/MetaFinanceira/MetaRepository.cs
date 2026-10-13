using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using CMWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.Infrastructure.Repositories
{
    public class MetaRepository : IMetaRepository
    {
        private readonly ApplicationDbContext _context;

        public MetaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarMetaAsync(MetaFinanceira meta, CancellationToken cancellationToken = default)
        {
            await _context.MetasFinanceiras.AddAsync(meta, cancellationToken);
        }

        public async Task<MetaFinanceira?> BuscarMetaParaEdicaoAsync(int metaId, CancellationToken cancellationToken = default)
        {
            return await _context.MetasFinanceiras.FirstOrDefaultAsync(m => m.MetaId == metaId, cancellationToken);
        }

        public async Task<MetaFinanceira?> BuscarMetaPorIdAsync(int metaId, CancellationToken cancellationToken = default)
        {
            return await _context.MetasFinanceiras
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MetaId == metaId, cancellationToken);
        }

        public async Task<List<MetaFinanceira>> ListarTodasMetasAsync(CancellationToken cancellationToken = default)
        {
            return await _context.MetasFinanceiras
                .AsNoTracking()
                .OrderBy(m => m.DataMeta)
                .ToListAsync(cancellationToken);
        }

        public void DeletarMeta(MetaFinanceira meta)
        {
            _context.MetasFinanceiras.Remove(meta);
        }

        public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
