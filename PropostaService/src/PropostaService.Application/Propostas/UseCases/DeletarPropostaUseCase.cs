using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class DeletarPropostaUseCase : IDeletarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;

        public DeletarPropostaUseCase(IPropostaRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ExecutarAsync(Guid id)
        {
            var proposta = await _repository.ObterPorIdAsync(id);
            if (proposta == null) 
                throw new NotFoundException($"A proposta com ID {id} não foi encontrada.");

            await _repository.DeletarAsync(id);
            return true;
        }
    }
}
