using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.Common.DTOs;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Application.Propostas.Interfaces;

namespace PropostaService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PropostasController : ControllerBase
    {
        private readonly ICriarPropostaUseCase _criarUseCase;
        private readonly IBuscarPropostaPorIdUseCase _obterUseCase;
        private readonly IBuscarTodasPropostasUseCase _buscarUseCase;
        private readonly IAtualizarPropostaUseCase _atualizarUseCase;
        private readonly IDeletarPropostaUseCase _deletarUseCase;
        private readonly IAprovarPropostaUseCase _aprovarUseCase;
        private readonly IRejeitarPropostaUseCase _rejeitarUseCase;
        public PropostasController(
            ICriarPropostaUseCase criarUseCase,
            IBuscarPropostaPorIdUseCase obterPorIdUseCase,
            IBuscarTodasPropostasUseCase obterTodasUseCase,
            IAtualizarPropostaUseCase atualizarUsecase,
            IDeletarPropostaUseCase deletarUsecase,
            IAprovarPropostaUseCase aprovarUseCase,
            IRejeitarPropostaUseCase rejeitarUseCase)
        {
            _criarUseCase = criarUseCase;
            _obterUseCase = obterPorIdUseCase;
            _buscarUseCase = obterTodasUseCase;
            _atualizarUseCase = atualizarUsecase;
            _deletarUseCase = deletarUsecase;
            _aprovarUseCase = aprovarUseCase;
            _rejeitarUseCase = rejeitarUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PropostaOutput), StatusCodes.Status201Created)]
        public async Task<IActionResult> Criar([FromBody] CriarPropostaInput input)
        {
          var output = await _criarUseCase.ExecutarAsync(input);
          return CreatedAtAction(
              nameof(ObterPorId),
              new { id = output.Id },
              output);
        }

        [HttpPost("{id}/aprovar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Aprovar(Guid id)
        {
            await _aprovarUseCase.ExecutarAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/rejeitar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Rejeitar(Guid id, [FromBody] RejeitarPropostaInput input)
        {
            await _rejeitarUseCase.ExecutarAsync(id, input);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropostaOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var output = await _obterUseCase.ExecutarAsync(id);

            return Ok(output);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListaPaginadaOutput<PropostaOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodos([FromQuery] BuscarPropostasInput input)
        {
            var resultado = await _buscarUseCase.ExecutarAsync(input);
            return Ok(resultado);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PropostaOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPropostaInput input)
        {
            if (id != input.Id) return BadRequest("O ID da URL difere do corpo.");
            var output = await _atualizarUseCase.ExecutarAsync(input);

            return Ok(output);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _deletarUseCase.ExecutarAsync(id);

            return NoContent();
        }
    }
}
