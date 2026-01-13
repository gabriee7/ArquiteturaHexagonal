using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;

namespace PropostaService.Application.Propostas.Interfaces
{
    public interface IAtualizarPropostaUseCase
    {
        Task<PropostaOutput> ExecutarAsync(AtualizarPropostaInput input);
    }
}
