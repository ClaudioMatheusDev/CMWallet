using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using CMWallet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.MetaFinanceira
{
    [ApiController]
    [Route("api/meta")]
    public class MetaController : ControllerBase
    {


        private readonly IMetaService _service;

        public MetaController(IMetaService service)
        {
            _service = service;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> CriarMeta([FromBody] MetaCriarDto dto)
        {
            var metaId = await _service.CriarMetaAsync(dto);

            return Ok(new
            {
                metaId = metaId
            });
        }

        [HttpGet("{metaId:int}")]
        public async Task<IActionResult> BuscarMetaPorId(int metaId)
        {
            var meta = await _service.BuscarMetaPorIdAsync(metaId);
            return Ok(meta);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarMetas()
        {
            var metas = await _service.ListarMetasAsync();
            return Ok(metas);
        }
        [HttpDelete("{metaId:int}")]
        public async Task<IActionResult> DeletarMeta(int metaId)
        {
            await _service.ApagarMetaAsync(metaId);
            return NoContent();
        }
        [HttpPut("{metaId:int}")]
        public async Task<IActionResult> AtualizarMeta(int metaId, [FromBody] MetaAtualizarDto dto)
        {
            await _service.AtualizarMetaAsync(metaId, dto);
            return NoContent();
        }

    }

}