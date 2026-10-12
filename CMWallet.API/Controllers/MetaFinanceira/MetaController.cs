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
        public async Task<IActionResult> CriarMeta([FromBody] MetaCriarDto dto, CancellationToken cancellationToken)
        {
            var metaId = await _service.CriarMetaAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(BuscarMetaPorId), new { metaId }, new { MetaId = metaId });
        }

        [HttpGet("{metaId:int}")]
        public async Task<ActionResult<MetaResponseDto>> BuscarMetaPorId(int metaId, CancellationToken cancellationToken)
        {
            return Ok(await _service.BuscarMetaPorIdAsync(metaId, cancellationToken));
        }

        [HttpGet]
        public async Task<ActionResult<List<MetaResponseDto>>> ListarMetas(CancellationToken cancellationToken)
        {
            return Ok(await _service.ListarMetasAsync(cancellationToken));
        }

        [HttpPut("{metaId:int}")]
        public async Task<IActionResult> AtualizarMeta(int metaId, [FromBody] MetaAtualizarDto dto, CancellationToken cancellationToken)
        {
            await _service.AtualizarMetaAsync(metaId, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{metaId:int}")]
        public async Task<IActionResult> DeletarMeta(int metaId, CancellationToken cancellationToken)
        {
            await _service.ApagarMetaAsync(metaId, cancellationToken);
            return NoContent();
        }
    }
}
