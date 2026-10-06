using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Transacao
{
    [ApiController]
    [Route("api/transacao")]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> CriarTransacao([FromBody] TransacaoCriarDto transacaoDto)
        {
            var transacaoId = await _transacaoService.CriarTransacaoAsync(transacaoDto);
            return CreatedAtAction(nameof(ListarTransacao), new { transacaoId }, new { TransacaoId = transacaoId });
        }

        [HttpGet("{transacaoId:int}")]
        public async Task<IActionResult> ListarTransacao([FromRoute] int transacaoId)
        {
            var transacao = await _transacaoService.BuscarTransacaoPorIdAsync(transacaoId);
            return Ok(transacao);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarTransacoes()
        {
            var transacoes = await _transacaoService.ListarTransacoesAsync();
            return Ok(transacoes);
        }

        [HttpDelete("{transacaoId:int}")]
        public async Task<IActionResult> DeletarTransacao([FromRoute] int transacaoId)
        {
            await _transacaoService.ApagarTransacaoAsync(transacaoId);
            return NoContent();
        }

        [HttpPut("{transacaoId:int}")]
        public async Task<IActionResult> AtualizarTransacao([FromRoute] int transacaoId, [FromBody] TransacaoAtualizarDto transacaoDto)
        {
            await _transacaoService.AtualizarTransacaoAsync(transacaoId, transacaoDto);
            return NoContent();
        }
    }
}
