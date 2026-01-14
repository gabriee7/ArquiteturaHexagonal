using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Ports.Repositories
{
    public interface IContratacaoRepository
    {
        Task<Contratacao> CriarAsync(Contratacao contratacao);
        Task<Contratacao?> ObterPorIdAsync(Guid id);
        Task<(List<Contratacao> itens, int total)> ObterTodosAsync(
          int pagina,
          int tamanhoPagina,
          string? termo,
          string ordenacao);
        Task DeletarAsync(Guid id);
        Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId);
    }
}
