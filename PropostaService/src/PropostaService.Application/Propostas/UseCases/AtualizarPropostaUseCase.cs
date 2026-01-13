using AutoMapper;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class AtualizarPropostaUseCase : IAtualizarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;
        private readonly IMapper _mapper;

        public AtualizarPropostaUseCase(IPropostaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PropostaOutput> ExecutarAsync(AtualizarPropostaInput input)
        {
            var proposta = await _repository.ObterPorIdAsync(input.Id);
            if (proposta == null)
                throw new NotFoundException($"Não é possível atualizar. Proposta {input.Id} não encontrada.");
            
            proposta.Update(input.NomeSegurado, input.Valor);
            await _repository.AtualizarAsync(proposta);
            return _mapper.Map<PropostaOutput>(proposta);
        }
    }
}
