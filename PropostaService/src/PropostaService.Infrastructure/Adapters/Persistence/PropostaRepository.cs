using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports.Repositories;
using PropostaService.Infrastructure.Data;

namespace PropostaService.Infrastructure.Adapters.Persistence
{
    public class PropostaRepository : IPropostaRepository
    {
        private readonly AppDbContext _context;

        public PropostaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Proposta> CriarAsync(Proposta proposta)
        {
            await _context.Propostas.AddAsync(proposta);
            await _context.SaveChangesAsync();
            return proposta;
        }

        public async Task<Proposta?> ObterPorIdAsync(Guid id)
        {
            return await _context.Propostas
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AtualizarAsync(Proposta proposta)
        {
            _context.Propostas.Update(proposta);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Proposta> itens, int total)> BuscarAsync(
            int pagina,
            int tamanhoPagina,
            string? termo,
            StatusPropostaEnum? status,
            string ordenacao)
        {
            var query = _context.Propostas.AsQueryable();
            query = query.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(termo))
                query = query.Where(p => p.NomeSegurado.Contains(termo));

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            var total = await query.CountAsync();

            query = ordenacao?.ToLower() == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);

            var itens = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (itens, total);
        }

        public async Task DeletarAsync(Guid id)
        {
            var proposta = await _context.Propostas.FindAsync(id);
            if (proposta != null)
            {
                proposta.Delete();
                _context.Propostas.Update(proposta);
                await _context.SaveChangesAsync();
            }
        }
    }
}
