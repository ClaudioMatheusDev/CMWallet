using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.MetaFinanceira
{
    [ApiController]
    [Route("api/metas")]
    public class MetaController : ControllerBase
    {
        private readonly IMetaService _service;

        public MetaController(IMetaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarMeta([FromBody] MetaCriarDto dto)
        {
            var metaId = await _service.CriarMetaAsync(dto);
            return CreatedAtAction(nameof(BuscarMetaPorId), new { metaId }, new { MetaId = metaId });
        }

        [HttpGet("{metaId:int}")]
        public async Task<ActionResult<MetaResponseDto>> BuscarMetaPorId(int metaId)
        {
            return Ok(await _service.BuscarMetaPorIdAsync(metaId));
        }

        [HttpGet]
        public async Task<ActionResult<List<MetaResponseDto>>> ListarMetas()
        {
            return Ok(await _service.ListarMetasAsync());
        }

        [HttpPut("{metaId:int}")]
        public async Task<IActionResult> AtualizarMeta(int metaId, [FromBody] MetaAtualizarDto dto)
        {
            await _service.AtualizarMetaAsync(metaId, dto);
            return NoContent();
        }

        [HttpDelete("{metaId:int}")]
        public async Task<IActionResult> DeletarMeta(int metaId)
        {
            await _service.ApagarMetaAsync(metaId);
            return NoContent();
        }
    }
}
