using PropostaService.Application.Propostas.DTOs.In;

namespace PropostaService.Application.Propostas.Interfaces
{
    public interface IRejeitarPropostaUseCase
    {
        Task ExecutarAsync(Guid id, RejeitarPropostaInput input);
    }
}
