using AutoMapper;
using ContratacaoService.Application.Contratacoes.DTOs.In;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.External.PropostaService;
using ContratacaoService.Domain.Ports.External.PropostaService.Enums;
using ContratacaoService.Domain.Ports.Repositories;

namespace ContratacaoService.Application.Contratacoes.UseCases
{
    public class CriarContratacaoUseCase : ICriarContratacaoUseCase
    {
        private readonly IContratacaoRepository _repository;
        private readonly IPropostaGateway _propostaGateway;
        private readonly IMapper _mapper;

        public CriarContratacaoUseCase(
          IContratacaoRepository repository,
          IPropostaGateway propostaGateway,
          IMapper mapper)
        {
            _repository = repository;
            _propostaGateway = propostaGateway;
            _mapper = mapper;
        }

        public async Task<ContratacaoOutput> ExecutarAsync(CriarContratacaoInput input)
        {
            var contratacaoExistente = await _repository.ObterPorPropostaIdAsync(input.PropostaId);
            if (contratacaoExistente != null)
                throw new DomainException("Esta proposta já foi contratada anteriormente.");
            var proposta = await _propostaGateway.ObterPorIdAsync(input.PropostaId);
            if (proposta == null)
                throw new DomainException($"Proposta {input.PropostaId} não encontrada.");
            if (proposta.Status != StatusPropostaEnum.Aprovada)
                throw new DomainException($"A proposta não pode ser contratada pois está com status: {proposta.Status}");

            var contratacao = new Contratacao(proposta.Id);
            await _repository.CriarAsync(contratacao);
            return _mapper.Map<ContratacaoOutput>(contratacao);
        }
    }
}
