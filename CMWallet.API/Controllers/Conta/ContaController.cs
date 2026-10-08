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
        public async Task<IActionResult> CriarConta([FromBody] ContaCriarDto contaDto)
        {
            var contaId = await _contaService.CriarContaAsync(contaDto);
            return CreatedAtAction(nameof(BuscarContaPorId), new { contaId }, new { ContaId = contaId });
        }

        [HttpGet("{contaId:int}")]
        public async Task<ActionResult<ContaResponseDto>> BuscarContaPorId(int contaId)
        {
            return Ok(await _contaService.BuscarContaPorIdAsync(contaId));
        }

        [HttpGet]
        public async Task<ActionResult<List<ContaResponseDto>>> ListarContas()
        {
            return Ok(await _contaService.ListarContasAsync());
        }

        [HttpPut("{contaId:int}")]
        public async Task<IActionResult> AtualizarConta(int contaId, [FromBody] ContaAtualizarDto contaDto)
        {
            await _contaService.AtualizarContaAsync(contaId, contaDto);
            return NoContent();
        }

        [HttpDelete("{contaId:int}")]
        public async Task<IActionResult> DeletarConta(int contaId)
        {
            await _contaService.ApagarContaAsync(contaId);
            return NoContent();
        }
    }
}
