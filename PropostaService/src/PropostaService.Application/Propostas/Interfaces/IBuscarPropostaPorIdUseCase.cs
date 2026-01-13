using PropostaService.Application.Propostas.DTOs.Out;

namespace PropostaService.Application.Propostas.Interfaces
{
    public interface IBuscarPropostaPorIdUseCase
    {
        Task<PropostaOutput> ExecutarAsync(Guid id);
    }
}
