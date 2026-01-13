using AutoMapper;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class CriarPropostaUseCase : ICriarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;
        private readonly IMapper _mapper;

        public CriarPropostaUseCase(
            IPropostaRepository repository, 
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PropostaOutput> ExecutarAsync(CriarPropostaInput input)
        {
            var proposta = new Proposta(input.NomeSegurado, input.Valor);
            await _repository.CriarAsync(proposta);
            return _mapper.Map<PropostaOutput>(proposta);
        }
    }
}
