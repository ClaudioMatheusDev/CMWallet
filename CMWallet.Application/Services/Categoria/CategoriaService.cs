using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<int> CriarCategoriaAsync(CategoriaCriarDto dto)
        {
            var categoria = new Categoria
            {
                Nome = dto.Nome,
                Tipo = dto.Tipo
            };

            await _categoriaRepository.AdicionarCategoriaAsync(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();

            return categoria.CategoriaId;
        }

        public async Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId);

            if (categoria == null)
            {
                throw new CategoriaNaoEncontradaException(categoriaId);
            }

            return new CategoriaResponseDto
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                Tipo = categoria.Tipo
            };
        }
        public async Task<List<CategoriaResponseDto>> BuscarTodasCategoriasAsync()
        {
            var categorias = await _categoriaRepository.ListarTodasCategorias();

            return categorias.Select(c => new CategoriaResponseDto
            {
                CategoriaId = c.CategoriaId,
                Nome = c.Nome,
                Tipo = c.Tipo
            }).ToList();
        }

        public async Task ApagarCategoriaAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId);

            if (categoria == null)
            {
                throw new CategoriaNaoEncontradaException(categoriaId);
            }

            _categoriaRepository.DeletarCategoria(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();


        }

        public async Task AtualizarCategoriaAsync(int categoriaId, CategoriaAtualizarDto dto)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId);

            if (categoria == null)
            {
                throw new CategoriaNaoEncontradaException(categoriaId);
            }

            categoria.Nome = dto.Nome;
            categoria.Tipo = dto.Tipo;

            _categoriaRepository.AtualizarCategoria(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();

        }
    }
}
