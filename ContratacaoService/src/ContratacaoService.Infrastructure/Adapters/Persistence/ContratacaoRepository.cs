using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports.Repositories;
using ContratacaoService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Adapters.Persistence
{
    public class ContratacaoRepository : IContratacaoRepository
    {
        private readonly AppDbContext _context;

        public ContratacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Contratacao> CriarAsync(Contratacao contratacao)
        {
            await _context.Contratacoes.AddAsync(contratacao);
            await _context.SaveChangesAsync();
            return contratacao;
        }

        public async Task<Contratacao?> ObterPorIdAsync(Guid id)
        {
            return await _context.Contratacoes
              .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(List<Contratacao> itens, int total)> ObterTodosAsync(
          int pagina,
          int tamanhoPagina,
          string? termo,
          string ordenacao)
        {
            var query = _context.Contratacoes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(termo) && Guid.TryParse(termo, out var propostaId))
                query = query.Where(c => c.PropostaId == propostaId);
            var total = await query.CountAsync();

            query = ordenacao?.ToLower() == "asc"
              ? query.OrderBy(c => c.CreatedAt)
              : query.OrderByDescending(c => c.CreatedAt);
            var itens = await query
              .Skip((pagina - 1) * tamanhoPagina)
              .Take(tamanhoPagina)
              .ToListAsync();
            return (itens, total);
        }

        public async Task DeletarAsync(Guid id)
        {
            var contratacao = await _context.Contratacoes.FindAsync(id);
            if (contratacao != null)
            {
                contratacao.Delete();
                _context.Contratacoes.Update(contratacao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId)
        {
            return await _context.Contratacoes
              .FirstOrDefaultAsync(c => c.PropostaId == propostaId);
        }
    }
}
