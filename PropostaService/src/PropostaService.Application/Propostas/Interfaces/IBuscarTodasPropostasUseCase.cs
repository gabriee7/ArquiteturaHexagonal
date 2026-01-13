using PropostaService.Application.Common.DTOs;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;

namespace PropostaService.Application.Propostas.Interfaces
{
    public interface IBuscarTodasPropostasUseCase
    {
        Task<ListaPaginadaOutput<PropostaOutput>> ExecutarAsync(BuscarPropostasInput input);
    }
}
