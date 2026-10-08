using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using CMWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarCategoriaAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
        }

        public async Task<Categoria?> BuscarCategoriaPorIdAsync(int categoriaId)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == categoriaId);
        }

        public async Task<List<Categoria>> ListarTodasCategorias()
        {
            return await _context.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        }

        public void DeletarCategoria(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
