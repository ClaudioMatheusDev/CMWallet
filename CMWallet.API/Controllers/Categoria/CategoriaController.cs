using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Categoria
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {

        private readonly ICategoriaService _service;

        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> CriarCategoria([FromBody] CategoriaCriarDto dto)
        {
            var categoriaId = await _service.CriarCategoriaAsync(dto);
            return Ok(new
            {
                CategoriaId = categoriaId
            });
        }
        [HttpGet("{categoriaId:int}")]
        public async Task<IActionResult> BuscarCategoriaPorId(int categoriaId)
        {
            var categoria = await _service.BuscarCategoriaPorIdAsync(categoriaId);
            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarCategorias()
        {
            var categorias = await _service.BuscarTodasCategoriasAsync();
            return Ok(categorias);
        }

        [HttpDelete("{categoriaId:int}")]
        public async Task<IActionResult> DeletarCategoria(int categoriaId)
        {
            var categoria = await _service.BuscarCategoriaPorIdAsync(categoriaId);
            await _service.ApagarCategoriaAsync(categoriaId);
            return NoContent();
        }

        [HttpPut("{categoriaId:int}")]
        public async Task<IActionResult> AtualizarCategoria(int categoriaId, [FromBody] CategoriaAtualizarDto dto)
        {
            var atualizado = await _service.AtualizarCategoriaAsync(categoriaId, dto);
            return NoContent();
        }

    }
}
