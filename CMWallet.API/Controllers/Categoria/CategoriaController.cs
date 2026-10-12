using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Categoria
{
    [ApiController]
    [Route("api/categorias")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarCategoria([FromBody] CategoriaCriarDto dto, CancellationToken cancellationToken)
        {
            var categoriaId = await _service.CriarCategoriaAsync(dto, cancellationToken);
            return CreatedAtAction(
                nameof(BuscarCategoriaPorId),
                new { categoriaId },
                new { CategoriaId = categoriaId });
        }

        [HttpGet("{categoriaId:int}")]
        public async Task<ActionResult<CategoriaResponseDto>> BuscarCategoriaPorId(int categoriaId, CancellationToken cancellationToken)
        {
            return Ok(await _service.BuscarCategoriaPorIdAsync(categoriaId, cancellationToken));
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaResponseDto>>> ListarCategorias(CancellationToken cancellationToken)
        {
            return Ok(await _service.BuscarTodasCategoriasAsync(cancellationToken));
        }

        [HttpPut("{categoriaId:int}")]
        public async Task<IActionResult> AtualizarCategoria(int categoriaId, [FromBody] CategoriaAtualizarDto dto, CancellationToken cancellationToken)
        {
            await _service.AtualizarCategoriaAsync(categoriaId, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{categoriaId:int}")]
        public async Task<IActionResult> DeletarCategoria(int categoriaId, CancellationToken cancellationToken)
        {
            await _service.ApagarCategoriaAsync(categoriaId, cancellationToken);
            return NoContent();
        }
    }
}
