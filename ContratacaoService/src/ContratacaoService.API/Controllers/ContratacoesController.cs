using Asp.Versioning;
using ContratacaoService.Application.Common.DTOs;
using ContratacaoService.Application.Contratacoes.DTOs.In;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ContratacoesController : ControllerBase
    {
        private readonly ICriarContratacaoUseCase _criarContratacaoUseCase;
        private readonly IObterContratacaoPorIdUseCase _obterContratacaoPorIdUseCase;
        private readonly IObterContratacaoPorPropostaIdUseCase _obterContratacaoPorPropostaIdUseCase;
        private readonly IBuscarTodasContratacoesUseCase _buscarTodasContratacoesUseCase;
        private readonly IDeletarContratacaoUseCase _deletarContratacaoUseCase;

        public ContratacoesController(
            ICriarContratacaoUseCase criarContratacaoUseCase,
            IObterContratacaoPorIdUseCase obterContratacaoPorIdUseCase,
            IObterContratacaoPorPropostaIdUseCase obterContratacaoPorPropostaIdUseCase,
            IBuscarTodasContratacoesUseCase buscarTodasContratacoesUseCase,
            IDeletarContratacaoUseCase deletarContratacaoUseCase)
        {
            _criarContratacaoUseCase = criarContratacaoUseCase;
            _obterContratacaoPorIdUseCase = obterContratacaoPorIdUseCase;
            _obterContratacaoPorPropostaIdUseCase = obterContratacaoPorPropostaIdUseCase;
            _buscarTodasContratacoesUseCase = buscarTodasContratacoesUseCase;
            _deletarContratacaoUseCase = deletarContratacaoUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContratacaoOutput), StatusCodes.Status201Created)]
        public async Task<IActionResult> Criar([FromBody] CriarContratacaoInput input)
        {
            var output = await _criarContratacaoUseCase.ExecutarAsync(input);
            return CreatedAtAction(nameof(ObterPorId), new { id = output.Id }, output);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ContratacaoOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var result = await _obterContratacaoPorIdUseCase.ExecutarAsync(id);
            return Ok(result);
        }

        [HttpGet("proposta/{propostaId:guid}")]
        [ProducesResponseType(typeof(ContratacaoOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorPropostaId(Guid propostaId)
        {
            var result = await _obterContratacaoPorPropostaIdUseCase.ExecutarAsync(propostaId);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListaPaginadaOutput<ContratacaoOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Buscar([FromQuery] BaseFiltroInput filtro)
        {
            var result = await _buscarTodasContratacoesUseCase.ExecutarAsync(filtro);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _deletarContratacaoUseCase.ExecutarAsync(id);
            return NoContent();
        }
    }
}