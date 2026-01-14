using ContratacaoService.Domain.Ports.External.PropostaService.DTOs;

namespace ContratacaoService.Domain.Ports.External.PropostaService
{
    public interface IPropostaGateway
    {
        Task<PropostaExternaDto?> ObterPorIdAsync(Guid id);
    }
}
