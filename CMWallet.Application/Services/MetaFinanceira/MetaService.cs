using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Services
{
    public class MetaService : IMetaService
    {
        private readonly IMetaRepository _metaRepository;
        private readonly IContaRepository _contaRepository;

        public MetaService(IMetaRepository metaRepository, IContaRepository contaRepository)
        {
            _metaRepository = metaRepository;
            _contaRepository = contaRepository;
        }

        public async Task<int> CriarMetaAsync(MetaCriarDto dto)
        {
            await GarantirContaExisteAsync(dto.ContaId);

            var meta = new MetaFinanceira
            {
                ValorMeta = dto.ValorMeta,
                ValorAtual = dto.ValorAtual,
                DataMeta = dto.DataMeta!.Value,
                ContaId = dto.ContaId
            };

            await _metaRepository.AdicionarMetaAsync(meta);
            await _metaRepository.SalvarAlteracoesAsync();

            return meta.MetaId;
        }

        public async Task<MetaResponseDto> BuscarMetaPorIdAsync(int metaId)
        {
            var meta = await ObterMetaAsync(metaId);

            return ParaDto(meta);
        }

        public async Task<List<MetaResponseDto>> ListarMetasAsync()
        {
            var metas = await _metaRepository.ListarTodasMetas();

            return metas.Select(ParaDto).ToList();
        }

        public async Task ApagarMetaAsync(int metaId)
        {
            var meta = await ObterMetaAsync(metaId);

            _metaRepository.DeletarMeta(meta);
            await _metaRepository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarMetaAsync(int metaId, MetaAtualizarDto dto)
        {
            var meta = await ObterMetaAsync(metaId);

            if (meta.ContaId != dto.ContaId)
            {
                await GarantirContaExisteAsync(dto.ContaId);
            }

            meta.ValorMeta = dto.ValorMeta;
            meta.ValorAtual = dto.ValorAtual;
            meta.DataMeta = dto.DataMeta!.Value;
            meta.ContaId = dto.ContaId;

            await _metaRepository.SalvarAlteracoesAsync();
        }

        private async Task<MetaFinanceira> ObterMetaAsync(int metaId)
        {
            return await _metaRepository.BuscarMetaPorIdAsync(metaId)
                ?? throw new MetaNaoEncontradaException(metaId);
        }

        private async Task GarantirContaExisteAsync(int contaId)
        {
            if (await _contaRepository.BuscarContaPorIdAsync(contaId) is null)
            {
                throw new ContaNaoEncontradaException(contaId);
            }
        }

        private static MetaResponseDto ParaDto(MetaFinanceira meta) => new()
        {
            MetaId = meta.MetaId,
            ValorMeta = meta.ValorMeta,
            ValorAtual = meta.ValorAtual,
            DataMeta = meta.DataMeta,
            ContaId = meta.ContaId
        };
    }
}
