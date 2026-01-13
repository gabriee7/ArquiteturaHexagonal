using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Ports.Repositories
{
    public interface IPropostaRepository
    {
        Task<Proposta> CriarAsync(Proposta proposta);
        Task<Proposta?> ObterPorIdAsync(Guid id);
        Task AtualizarAsync(Proposta proposta);
        Task<(List<Proposta> itens, int total)> BuscarAsync(
            int pagina,
            int tamanhoPagina,
            string? termo,
            StatusPropostaEnum? status,
            string ordenacao);
        Task DeletarAsync(Guid id);
    }
}
