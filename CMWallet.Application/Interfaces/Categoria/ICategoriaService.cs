using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<int> CriarCategoriaAsync(CategoriaCriarDto dto);
        Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int categoriaId);
        Task<List<CategoriaResponseDto>> BuscarTodasCategoriasAsync();
        Task<bool> ApagarCategoriaAsync(int categoriaId);
        Task<bool> AtualizarCategoriaAsync(int categoriaID, CategoriaAtualizarDto dto);
    }
}
