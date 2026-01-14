using ContratacaoService.Application.Contratacoes.DTOs.Out;

namespace ContratacaoService.Application.Contratacoes.Interfaces
{
    public interface IObterContratacaoPorIdUseCase
    {
        Task<ContratacaoOutput> ExecutarAsync(Guid id);
    }
}
