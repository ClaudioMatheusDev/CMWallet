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

        public async Task AdicionarCategoriaAsync(Categoria categoria, CancellationToken cancellationToken = default)
        {
            await _context.Categorias.AddAsync(categoria, cancellationToken);
        }

        public async Task<Categoria?> BuscarCategoriaParaEdicaoAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == categoriaId, cancellationToken);
        }

        public async Task<Categoria?> BuscarCategoriaPorIdAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            return await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoriaId == categoriaId, cancellationToken);
        }

        public async Task<List<Categoria>> ListarTodasCategoriasAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> ContarTransacoesAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            return await _context.Transacoes.CountAsync(t => t.CategoriaId == categoriaId, cancellationToken);
        }

        public void DeletarCategoria(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
        }

        public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
