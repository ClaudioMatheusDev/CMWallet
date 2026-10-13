using CMWallet.Application.Dtos;
using CMWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMWallet.API.Controllers.Resumo
{
    [ApiController]
    [Route("api/resumo")]
    public class ResumoController : ControllerBase
    {
        private readonly IResumoService _resumoService;

        public ResumoController(IResumoService resumoService)
        {
            _resumoService = resumoService;
        }

        /// <summary>Totais de entradas, saídas e pendências do período, com a composição por categoria.</summary>
        /// <remarks>Sem datas, considera o mês corrente.</remarks>
        [HttpGet]
        public async Task<ActionResult<ResumoResponseDto>> ObterResumo(
            [FromQuery] ResumoFiltroDto filtro,
            CancellationToken cancellationToken)
        {
            return Ok(await _resumoService.ObterResumoAsync(filtro, cancellationToken));
        }
    }
}
