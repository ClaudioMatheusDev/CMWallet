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

        public async Task<int> CriarCategoriaAsync(CategoriaCriarDto dto, CancellationToken cancellationToken = default)
        {
            var categoria = new Categoria
            {
                Nome = dto.Nome.Trim(),
                Tipo = dto.Tipo
            };

            await _categoriaRepository.AdicionarCategoriaAsync(categoria, cancellationToken);
            await _categoriaRepository.SalvarAlteracoesAsync(cancellationToken);

            return categoria.CategoriaId;
        }

        public async Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaRepository.BuscarCategoriaPorIdAsync(categoriaId, cancellationToken)
                ?? throw new CategoriaNaoEncontradaException(categoriaId);

            return ParaDto(categoria);
        }

        public async Task<List<CategoriaResponseDto>> BuscarTodasCategoriasAsync(CancellationToken cancellationToken = default)
        {
            var categorias = await _categoriaRepository.ListarTodasCategoriasAsync(cancellationToken);
            return categorias.Select(ParaDto).ToList();
        }

        public async Task ApagarCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            var categoria = await ObterParaEdicaoAsync(categoriaId, cancellationToken);

            var transacoes = await _categoriaRepository.ContarTransacoesAsync(categoriaId, cancellationToken);
            if (transacoes > 0)
            {
                throw new ConflitoNegocioException(
                    $"A categoria \"{categoria.Nome}\" é usada em " +
                    $"{Texto.Quantidade(transacoes, "transação", "transações")}. " +
                    "Altere a categoria desses lançamentos antes de excluí-la.");
            }

            _categoriaRepository.DeletarCategoria(categoria);
            await _categoriaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        public async Task AtualizarCategoriaAsync(int categoriaId, CategoriaAtualizarDto dto, CancellationToken cancellationToken = default)
        {
            var categoria = await ObterParaEdicaoAsync(categoriaId, cancellationToken);

            // Mudar o tipo deixaria as transações já lançadas com tipo diferente da categoria.
            if (categoria.Tipo != dto.Tipo
                && await _categoriaRepository.ContarTransacoesAsync(categoriaId, cancellationToken) > 0)
            {
                throw new ConflitoNegocioException(
                    $"A categoria \"{categoria.Nome}\" já possui transações; o tipo dela não pode ser alterado.");
            }

            categoria.Nome = dto.Nome.Trim();
            categoria.Tipo = dto.Tipo;

            await _categoriaRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        private async Task<Categoria> ObterParaEdicaoAsync(int categoriaId, CancellationToken cancellationToken)
        {
            return await _categoriaRepository.BuscarCategoriaParaEdicaoAsync(categoriaId, cancellationToken)
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
