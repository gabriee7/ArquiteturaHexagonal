using AutoMapper;
using ContratacaoService.Application.Common.DTOs;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.Repositories;

namespace ContratacaoService.Application.Contratacoes.UseCases
{
    public class BuscarTodasContratacoesUseCase : IBuscarTodasContratacoesUseCase
    {
        private readonly IContratacaoRepository _repository;
        private readonly IMapper _mapper;

        public BuscarTodasContratacoesUseCase(IContratacaoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ListaPaginadaOutput<ContratacaoOutput>> ExecutarAsync(BaseFiltroInput input)
        {
            var (itens, total) = await _repository.ObterTodosAsync(
              input.Pagina,
              input.TamanhoPagina,
              input.TermoBusca,
              input.Ordenacao);

            var outputItens = _mapper.Map<List<ContratacaoOutput>>(itens);
            return new ListaPaginadaOutput<ContratacaoOutput>(
              outputItens,
              total,
              input.Pagina,
              input.TamanhoPagina);
        }
    }
}
