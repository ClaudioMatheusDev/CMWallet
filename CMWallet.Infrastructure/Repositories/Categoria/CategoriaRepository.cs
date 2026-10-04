using CMWallet.Application.Interfaces;
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

        public async Task AdicionarCategoriaAsync(Domain.Entities.Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
        }

        public async Task<Domain.Entities.Categoria?> BuscarCategoriaPorIdAsync(int categoriaId)
        {
           return await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == categoriaId);
        }

        public async Task<List<Domain.Entities.Categoria>> ListarTodasCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }
        public void AtualizarCategoria(Domain.Entities.Categoria categoria)
        {
             _context.Categorias.Update(categoria);
        }

        public void DeletarCategoria(Domain.Entities.Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }

   }
}
