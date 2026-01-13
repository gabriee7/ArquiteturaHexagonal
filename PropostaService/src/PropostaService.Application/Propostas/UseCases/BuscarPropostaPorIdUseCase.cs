using AutoMapper;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class BuscarPropostaPorIdUseCase : IBuscarPropostaPorIdUseCase
    {
        private readonly IPropostaRepository _repository;
        private readonly IMapper _mapper;

        public BuscarPropostaPorIdUseCase(IPropostaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PropostaOutput> ExecutarAsync(Guid id)
        {
            var proposta = await _repository.ObterPorIdAsync(id);
            if (proposta == null)
                throw new NotFoundException($"A proposta com ID {id} não foi encontrada.");

            return _mapper.Map<PropostaOutput>(proposta);
        }
    }
}
