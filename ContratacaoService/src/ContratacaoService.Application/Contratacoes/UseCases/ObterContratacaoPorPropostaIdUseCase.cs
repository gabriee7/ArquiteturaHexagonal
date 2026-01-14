using AutoMapper;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.Repositories;

namespace ContratacaoService.Application.Contratacoes.UseCases
{
    public class ObterContratacaoPorPropostaIdUseCase : IObterContratacaoPorPropostaIdUseCase
    {
        private readonly IContratacaoRepository _repository;
        private readonly IMapper _mapper;

        public ObterContratacaoPorPropostaIdUseCase(
          IContratacaoRepository repository,
          IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContratacaoOutput?> ExecutarAsync(Guid propostaId)
        {
            var contratacao = await _repository.ObterPorPropostaIdAsync(propostaId);
            if (contratacao == null)
                throw new NotFoundException("Contratação não encontrada.");

            return _mapper.Map<ContratacaoOutput>(contratacao);
        }
    }
}
