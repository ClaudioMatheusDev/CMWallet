using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Conta
{
    [ApiController]
    [Route("api/conta")]
    public class ContaController : ControllerBase
    {

        private readonly IContaService _contaService;

        public ContaController(IContaService contaService)
        {
            _contaService = contaService;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> CreateConta([FromBody] ContaCriarDto contaDto)
        {
            var ContaId = await _contaService.CriarContaAsync(contaDto);


            return Ok(new
            {
                ContaId = ContaId
            });
        }

        [HttpGet("{contaId:int}")]
        public async Task<IActionResult> GetContaById(int contaId)
        {
            var conta = await _contaService.BuscarContaPorIdAsync(contaId);
            if (conta == null)
            {
                return NotFound();
            }
            return Ok(conta);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarContas()
        {
            var contas = await _contaService.ListarContasAsync();
            return Ok(contas);
        }

        [HttpDelete("{contaId:int}")]
        public async Task<IActionResult> DeletarConta(int contaId)
        {
            var conta = await _contaService.BuscarContaPorIdAsync(contaId);

            if (conta == null)
            {
                return NotFound();
            }

            await _contaService.ApagarContaAsync(contaId);

            return Ok(new { Mensagem = "Conta deletada com sucesso." });
        }
        [HttpPut("{contaId:int}")]
        public async Task<IActionResult> AtualizarConta(int contaId, [FromBody] ContaAtualizarDto contaDto)
        {
            var contaExistente = await _contaService.BuscarContaPorIdAsync(contaId);
            if (contaExistente == null)
            {
                return NotFound();
            }
            await _contaService.AtualizarContaAsync(contaId, contaDto);
            return Ok(new { Mensagem = "Conta atualizada com sucesso." });
        }
    }
}
