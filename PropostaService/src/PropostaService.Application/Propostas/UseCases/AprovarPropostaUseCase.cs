using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class AprovarPropostaUseCase : IAprovarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;

        public AprovarPropostaUseCase(IPropostaRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecutarAsync(Guid id)
        {
            var proposta = await _repository.ObterPorIdAsync(id);
            if (proposta == null) 
                throw new NotFoundException($"Proposta {id} não encontrada.");
            
            proposta.Aprovar();
            await _repository.AtualizarAsync(proposta);
        }
    }
}
