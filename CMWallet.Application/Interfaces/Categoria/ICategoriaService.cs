using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<int> CriarCategoriaAsync(CategoriaCriarDto dto, CancellationToken cancellationToken = default);
        Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int categoriaId, CancellationToken cancellationToken = default);
        Task<List<CategoriaResponseDto>> BuscarTodasCategoriasAsync(CancellationToken cancellationToken = default);
        Task ApagarCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default);
        Task AtualizarCategoriaAsync(int categoriaId, CategoriaAtualizarDto dto, CancellationToken cancellationToken = default);
    }
}
