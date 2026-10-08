using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Transacao
{
    [ApiController]
    [Route("api/transacoes")]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarTransacao([FromBody] TransacaoCriarDto transacaoDto)
        {
            var transacaoId = await _transacaoService.CriarTransacaoAsync(transacaoDto);
            return CreatedAtAction(nameof(BuscarTransacaoPorId), new { transacaoId }, new { TransacaoId = transacaoId });
        }

        [HttpGet("{transacaoId:int}")]
        public async Task<ActionResult<TransacaoResponseDto>> BuscarTransacaoPorId(int transacaoId)
        {
            return Ok(await _transacaoService.BuscarTransacaoPorIdAsync(transacaoId));
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TransacaoResponseDto>>> ListarTransacoes([FromQuery] TransacaoFiltroDto filtro)
        {
            return Ok(await _transacaoService.ListarTransacoesAsync(filtro));
        }

        [HttpPut("{transacaoId:int}")]
        public async Task<IActionResult> AtualizarTransacao(int transacaoId, [FromBody] TransacaoAtualizarDto transacaoDto)
        {
            await _transacaoService.AtualizarTransacaoAsync(transacaoId, transacaoDto);
            return NoContent();
        }

        [HttpDelete("{transacaoId:int}")]
        public async Task<IActionResult> DeletarTransacao(int transacaoId)
        {
            await _transacaoService.ApagarTransacaoAsync(transacaoId);
            return NoContent();
        }
    }
}
