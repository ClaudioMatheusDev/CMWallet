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
        public async Task<IActionResult> CriarCategoria([FromBody] CategoriaCriarDto dto)
        {
            var categoriaId = await _service.CriarCategoriaAsync(dto);
            return CreatedAtAction(
                nameof(BuscarCategoriaPorId),
                new { categoriaId },
                new { CategoriaId = categoriaId });
        }

        [HttpGet("{categoriaId:int}")]
        public async Task<ActionResult<CategoriaResponseDto>> BuscarCategoriaPorId(int categoriaId)
        {
            return Ok(await _service.BuscarCategoriaPorIdAsync(categoriaId));
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaResponseDto>>> ListarCategorias()
        {
            return Ok(await _service.BuscarTodasCategoriasAsync());
        }

        [HttpPut("{categoriaId:int}")]
        public async Task<IActionResult> AtualizarCategoria(int categoriaId, [FromBody] CategoriaAtualizarDto dto)
        {
            await _service.AtualizarCategoriaAsync(categoriaId, dto);
            return NoContent();
        }

        [HttpDelete("{categoriaId:int}")]
        public async Task<IActionResult> DeletarCategoria(int categoriaId)
        {
            await _service.ApagarCategoriaAsync(categoriaId);
            return NoContent();
        }
    }
}
