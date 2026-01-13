using AutoMapper;
using PropostaService.Application.Common.DTOs;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.Application.Propostas.UseCases
{
    public class BuscarTodasPropostasUseCase : IBuscarTodasPropostasUseCase
    {
        private readonly IPropostaRepository _repository;
        private readonly IMapper _mapper;

        public BuscarTodasPropostasUseCase(IPropostaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ListaPaginadaOutput<PropostaOutput>> ExecutarAsync(BuscarPropostasInput input)
        {
            var (itens, total) = await _repository.BuscarAsync(
                input.Pagina,
                input.TamanhoPagina,
                input.TermoBusca,
                input.Status,
                input.Ordenacao);

            var outputItens = _mapper.Map<List<PropostaOutput>>(itens);

            return new ListaPaginadaOutput<PropostaOutput>(outputItens, total, input.Pagina, input.TamanhoPagina);
        }
    }
}
