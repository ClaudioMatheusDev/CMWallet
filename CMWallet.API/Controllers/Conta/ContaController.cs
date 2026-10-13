using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Conta
{
    [ApiController]
    [Route("api/contas")]
    public class ContaController : ControllerBase
    {
        private readonly IContaService _contaService;

        public ContaController(IContaService contaService)
        {
            _contaService = contaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] ContaCriarDto contaDto, CancellationToken cancellationToken)
        {
            var contaId = await _contaService.CriarContaAsync(contaDto, cancellationToken);
            return CreatedAtAction(nameof(BuscarContaPorId), new { contaId }, new { ContaId = contaId });
        }

        [HttpGet("{contaId:int}")]
        public async Task<ActionResult<ContaResponseDto>> BuscarContaPorId(int contaId, CancellationToken cancellationToken)
        {
            return Ok(await _contaService.BuscarContaPorIdAsync(contaId, cancellationToken));
        }

        [HttpGet]
        public async Task<ActionResult<List<ContaResponseDto>>> ListarContas(CancellationToken cancellationToken)
        {
            return Ok(await _contaService.ListarContasAsync(cancellationToken));
        }

        [HttpPut("{contaId:int}")]
        public async Task<IActionResult> AtualizarConta(int contaId, [FromBody] ContaAtualizarDto contaDto, CancellationToken cancellationToken)
        {
            await _contaService.AtualizarContaAsync(contaId, contaDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{contaId:int}")]
        public async Task<IActionResult> DeletarConta(int contaId, CancellationToken cancellationToken)
        {
            await _contaService.ApagarContaAsync(contaId, cancellationToken);
            return NoContent();
        }
    }
}
