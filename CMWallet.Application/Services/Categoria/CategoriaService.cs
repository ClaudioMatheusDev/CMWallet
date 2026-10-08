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
                Nome = dto.Nome.Trim(),
                Tipo = dto.Tipo
            };

            await _categoriaRepository.AdicionarCategoriaAsync(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();

            return categoria.CategoriaId;
        }

        public async Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int categoriaId)
        {
            var categoria = await ObterCategoriaAsync(categoriaId);

            return ParaDto(categoria);
        }

        public async Task<List<CategoriaResponseDto>> BuscarTodasCategoriasAsync()
        {
            var categorias = await _categoriaRepository.ListarTodasCategorias();

            return categorias.Select(ParaDto).ToList();
        }

        public async Task ApagarCategoriaAsync(int categoriaId)
        {
            var categoria = await ObterCategoriaAsync(categoriaId);

            _categoriaRepository.DeletarCategoria(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarCategoriaAsync(int categoriaId, CategoriaAtualizarDto dto)
        {
            var categoria = await ObterCategoriaAsync(categoriaId);

            categoria.Nome = dto.Nome.Trim();
            categoria.Tipo = dto.Tipo;

            await _categoriaRepository.SalvarAlteracoesAsync();
        }

        private async Task<Categoria> ObterCategoriaAsync(int categoriaId)
        {
            return await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId)
                ?? throw new CategoriaNaoEncontradaException(categoriaId);
        }

        private static CategoriaResponseDto ParaDto(Categoria categoria) => new()
        {
            CategoriaId = categoria.CategoriaId,
            Nome = categoria.Nome,
            Tipo = categoria.Tipo
        };
    }
}
