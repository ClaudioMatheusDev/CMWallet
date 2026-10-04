using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using System.Runtime.InteropServices;

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
                throw new Exception("Categoria não encontrada.");
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

        public async Task<bool> ApagarCategoriaAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId);

            if (categoria == null)
            {
                throw new Exception("Categoria não encontrada.");
            }

            _categoriaRepository.DeletarCategoria(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();


            return true;
        }

        public async Task<bool> AtualizarCategoriaAsync(int categoriaID, CategoriaAtualizarDto dto)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaID);

            if (categoria == null)
            {
                throw new Exception("Categoria não encontrada.");
            }

            categoria.Nome = dto.Nome;
            categoria.Tipo = dto.Tipo;

            _categoriaRepository.AtualizarCategoria(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();

            return true;
        }
    }
}
