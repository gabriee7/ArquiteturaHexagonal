using AutoMapper;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.Repositories;

namespace ContratacaoService.Application.Contratacoes.UseCases
{
    public class ObterContratacaoPorIdUseCase : IObterContratacaoPorIdUseCase
    {
        private readonly IContratacaoRepository _repository;
        private readonly IMapper _mapper;

        public ObterContratacaoPorIdUseCase(
          IContratacaoRepository repository,
          IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContratacaoOutput?> ExecutarAsync(Guid id)
        {
            var contratacao = await _repository.ObterPorIdAsync(id);
            if(contratacao == null)
                throw new NotFoundException("Contratação não encontrada.");

            return _mapper.Map<ContratacaoOutput>(contratacao);
        }
    }
}
