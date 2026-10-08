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

        public async Task AdicionarMetaAsync(MetaFinanceira meta)
        {
            await _context.MetasFinanceiras.AddAsync(meta);
        }

        public async Task<MetaFinanceira?> BuscarMetaPorIdAsync(int metaId)
        {
            return await _context.MetasFinanceiras.FirstOrDefaultAsync(m => m.MetaId == metaId);
        }

        public async Task<List<MetaFinanceira>> ListarTodasMetas()
        {
            return await _context.MetasFinanceiras.AsNoTracking().OrderBy(m => m.DataMeta).ToListAsync();
        }

        public void DeletarMeta(MetaFinanceira meta)
        {
            _context.MetasFinanceiras.Remove(meta);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
