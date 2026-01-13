using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class RejeitarPropostaUseCase : IRejeitarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;

        public RejeitarPropostaUseCase(IPropostaRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecutarAsync(Guid id, RejeitarPropostaInput input)
        {
            var proposta = await _repository.ObterPorIdAsync(id);
            if (proposta == null) 
                throw new NotFoundException($"Proposta {id} não encontrada.");

            proposta.Rejeitar(input.Motivo);
            await _repository.AtualizarAsync(proposta);
        }
    }
}
