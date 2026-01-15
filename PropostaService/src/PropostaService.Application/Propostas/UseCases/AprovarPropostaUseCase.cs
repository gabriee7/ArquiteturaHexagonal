using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Ports.Messaging;
using PropostaService.Domain.Ports.Messaging.Events;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class AprovarPropostaUseCase : IAprovarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;
        private readonly IMessageBus _messageBus;

        public AprovarPropostaUseCase(
            IPropostaRepository repository,
            IMessageBus messageBus)
        {
            _repository = repository;
            _messageBus = messageBus;
        }

        public async Task ExecutarAsync(Guid id)
        {
            var proposta = await _repository.ObterPorIdAsync(id);
            if (proposta == null) 
                throw new NotFoundException($"Proposta {id} não encontrada.");
            
            proposta.Aprovar();
            await _repository.AtualizarAsync(proposta);
            await _messageBus.PublishAsync(new PropostaAprovadaEvent(
                proposta.Id,
                proposta.Valor,
                proposta.NomeSegurado));
        }
    }
}
