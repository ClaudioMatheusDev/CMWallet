using CMWallet.Application.Dtos;
using CMWallet.Application.Exceptions;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;

namespace CMWallet.Application.Services
{
    public class MetaService : IMetaService
    {

        private readonly IMetaRepository _repository;

        public MetaService(IMetaRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CriarMetaAsync(MetaCriarDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ValorMeta.ToString()))
            {
                throw new ValidacaoNegocioException("O valor da meta é obrigatório.");
            }

            var meta = new MetaFinanceira
            {
                ValorMeta = dto.ValorMeta,
                ValorAtual = dto.ValorAtual,
                DataMeta = dto.DataMeta,
                ContaId = dto.ContaId
            };

            await _repository.AdicionarMetaAsync(meta);
            await _repository.SalvarAlteracoesAsync();

            return meta.MetaId;
        }

        public async Task<MetaResponseDto> BuscarMetaPorIdAsync(int metaId)
        {
            var meta = await _repository.BuscarMetasPorIdAsync(metaId);

            if (meta == null)
            {
                throw new Exception($"Meta com ID {metaId} não encontrada.");
            }

            return new MetaResponseDto
            {
                MetaId = meta.MetaId,
                ValorMeta = meta.ValorMeta,
                ValorAtual = meta.ValorAtual,
                DataMeta = meta.DataMeta,
                ContaId = meta.ContaId
            };
        }

        public async Task<List<MetaResponseDto>> ListarMetasAsync()
        {
            var metas = await _repository.ListarTodasMetas();

            return metas.Select(metas => new MetaResponseDto
            {
                MetaId = metas.MetaId,
                ValorMeta = metas.ValorMeta,
                ValorAtual = metas.ValorAtual,
                DataMeta = metas.DataMeta,
                ContaId = metas.ContaId
            }).ToList();
        }

        public async Task ApagarMetaAsync(int MetaId)
        {
            var meta = await _repository.BuscarMetasPorIdAsync(MetaId);

            if (meta == null)
            {
                throw new Exception("MetaId não encontrado.");
            }

            _repository.DeletarMeta(meta);
            await _repository.SalvarAlteracoesAsync();
        }

        public async Task AtualizarMetaAsync(int MetaId, MetaAtualizarDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DataMeta.ToString()))
            {
                throw new ValidacaoNegocioException("A data da meta é obrigatória.");
            }

            var meta = await _repository.BuscarMetasPorIdAsync(MetaId);

            if (meta == null)
            {
                throw new Exception($"Meta com ID {MetaId} não encontrada.");
            }

            meta.ValorMeta = dto.ValorMeta;
            meta.ValorAtual = dto.ValorAtual;
            meta.DataMeta = dto.DataMeta;
            meta.ContaId = dto.ContaId;

            _repository.AtualizarMeta(meta);
            await _repository.SalvarAlteracoesAsync();

        }

    }
}
