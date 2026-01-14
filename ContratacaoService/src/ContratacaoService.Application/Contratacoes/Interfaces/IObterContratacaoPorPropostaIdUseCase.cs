using ContratacaoService.Application.Contratacoes.DTOs.Out;

namespace ContratacaoService.Application.Contratacoes.Interfaces
{
    public interface IObterContratacaoPorPropostaIdUseCase
    {
        Task<ContratacaoOutput> ExecutarAsync(Guid propostaId);
    }
}
