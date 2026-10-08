using CMWallet.Application.Dtos;

namespace CMWallet.Application.Interfaces
{
    public interface IMetaService
    {
        Task<int> CriarMetaAsync(MetaCriarDto dto);
        Task<MetaResponseDto> BuscarMetaPorIdAsync(int metaId);
        Task<List<MetaResponseDto>> ListarMetasAsync();
        Task ApagarMetaAsync(int MetaId);
        Task AtualizarMetaAsync(int MetaId, MetaAtualizarDto dto);
    }
}
