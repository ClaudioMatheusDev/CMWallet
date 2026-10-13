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

        public async Task<int> CriarMetaAsync(MetaCriarDto dto, CancellationToken cancellationToken = default)
        {
            await GarantirContaExisteAsync(dto.ContaId, cancellationToken);

            var meta = new MetaFinanceira
            {
                ValorMeta = dto.ValorMeta,
                ValorAtual = dto.ValorAtual,
                DataMeta = dto.DataMeta!.Value,
                ContaId = dto.ContaId
            };

            await _metaRepository.AdicionarMetaAsync(meta, cancellationToken);
            await _metaRepository.SalvarAlteracoesAsync(cancellationToken);

            return meta.MetaId;
        }

        public async Task<MetaResponseDto> BuscarMetaPorIdAsync(int metaId, CancellationToken cancellationToken = default)
        {
            var meta = await _metaRepository.BuscarMetaPorIdAsync(metaId, cancellationToken)
                ?? throw new MetaNaoEncontradaException(metaId);

            return ParaDto(meta);
        }

        public async Task<List<MetaResponseDto>> ListarMetasAsync(CancellationToken cancellationToken = default)
        {
            var metas = await _metaRepository.ListarTodasMetasAsync(cancellationToken);
            return metas.Select(ParaDto).ToList();
        }

        public async Task ApagarMetaAsync(int metaId, CancellationToken cancellationToken = default)
        {
            var meta = await ObterParaEdicaoAsync(metaId, cancellationToken);

            _metaRepository.DeletarMeta(meta);
            await _metaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        public async Task AtualizarMetaAsync(int metaId, MetaAtualizarDto dto, CancellationToken cancellationToken = default)
        {
            var meta = await ObterParaEdicaoAsync(metaId, cancellationToken);

            if (meta.ContaId != dto.ContaId)
            {
                await GarantirContaExisteAsync(dto.ContaId, cancellationToken);
            }

            meta.ValorMeta = dto.ValorMeta;
            meta.ValorAtual = dto.ValorAtual;
            meta.DataMeta = dto.DataMeta!.Value;
            meta.ContaId = dto.ContaId;

            await _metaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        private async Task<MetaFinanceira> ObterParaEdicaoAsync(int metaId, CancellationToken cancellationToken)
        {
            return await _metaRepository.BuscarMetaParaEdicaoAsync(metaId, cancellationToken)
                ?? throw new MetaNaoEncontradaException(metaId);
        }

        private async Task GarantirContaExisteAsync(int contaId, CancellationToken cancellationToken)
        {
            if (!await _contaRepository.ExisteContaAsync(contaId, cancellationToken))
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
