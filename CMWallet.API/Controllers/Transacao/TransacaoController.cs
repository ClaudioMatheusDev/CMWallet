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
        public async Task<IActionResult> CriarTransacao([FromBody] TransacaoCriarDto transacaoDto, CancellationToken cancellationToken)
        {
            var transacaoId = await _transacaoService.CriarTransacaoAsync(transacaoDto, cancellationToken);
            return CreatedAtAction(nameof(BuscarTransacaoPorId), new { transacaoId }, new { TransacaoId = transacaoId });
        }

        [HttpGet("{transacaoId:int}")]
        public async Task<ActionResult<TransacaoResponseDto>> BuscarTransacaoPorId(int transacaoId, CancellationToken cancellationToken)
        {
            return Ok(await _transacaoService.BuscarTransacaoPorIdAsync(transacaoId, cancellationToken));
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TransacaoResponseDto>>> ListarTransacoes(
            [FromQuery] TransacaoFiltroDto filtro,
            CancellationToken cancellationToken)
        {
            return Ok(await _transacaoService.ListarTransacoesAsync(filtro, cancellationToken));
        }

        [HttpPut("{transacaoId:int}")]
        public async Task<IActionResult> AtualizarTransacao(int transacaoId, [FromBody] TransacaoAtualizarDto transacaoDto, CancellationToken cancellationToken)
        {
            await _transacaoService.AtualizarTransacaoAsync(transacaoId, transacaoDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{transacaoId:int}")]
        public async Task<IActionResult> DeletarTransacao(int transacaoId, CancellationToken cancellationToken)
        {
            await _transacaoService.ApagarTransacaoAsync(transacaoId, cancellationToken);
            return NoContent();
        }
    }
}
