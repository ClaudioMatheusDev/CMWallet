using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface IResumoService
    {
        Task<ResumoResponseDto> ObterResumoAsync(ResumoFiltroDto filtro, CancellationToken cancellationToken = default);
    }
}
