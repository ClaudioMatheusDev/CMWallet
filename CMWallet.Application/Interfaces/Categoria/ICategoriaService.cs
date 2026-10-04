using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<int> CriarCategoriaAsync(CategoriaCriarDto dto);
        Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int categoriaId);
        Task<List<CategoriaResponseDto>> BuscarTodasCategoriasAsync();
        Task ApagarCategoriaAsync(int categoriaId);
        Task AtualizarCategoriaAsync(int categoriaId, CategoriaAtualizarDto dto);
    }
}
