using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface IMetaService
    {
        Task<int> CriarMetaAsync(MetaCriarDto dto, CancellationToken cancellationToken = default);
        Task<MetaResponseDto> BuscarMetaPorIdAsync(int metaId, CancellationToken cancellationToken = default);
        Task<List<MetaResponseDto>> ListarMetasAsync(CancellationToken cancellationToken = default);
        Task ApagarMetaAsync(int metaId, CancellationToken cancellationToken = default);
        Task AtualizarMetaAsync(int metaId, MetaAtualizarDto dto, CancellationToken cancellationToken = default);
    }
}
