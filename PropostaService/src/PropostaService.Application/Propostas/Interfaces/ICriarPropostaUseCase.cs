using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;

namespace PropostaService.Application.Propostas.Interfaces
{
    public interface ICriarPropostaUseCase
    {
        Task<PropostaOutput> ExecutarAsync(CriarPropostaInput input);
    }
}
