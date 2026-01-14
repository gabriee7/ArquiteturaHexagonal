using ContratacaoService.Application.Contratacoes.DTOs.In;
using ContratacaoService.Application.Contratacoes.DTOs.Out;

namespace ContratacaoService.Application.Contratacoes.Interfaces
{
    public interface ICriarContratacaoUseCase
    {
        Task<ContratacaoOutput> ExecutarAsync(CriarContratacaoInput input);
    }
}
