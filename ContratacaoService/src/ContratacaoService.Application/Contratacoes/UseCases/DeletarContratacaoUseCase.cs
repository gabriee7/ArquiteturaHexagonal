using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.Repositories;

namespace ContratacaoService.Application.Contratacoes.UseCases
{
    public class DeletarContratacaoUseCase : IDeletarContratacaoUseCase
    {
        private readonly IContratacaoRepository _repository;

        public DeletarContratacaoUseCase(IContratacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecutarAsync(Guid id)
        {
            var contratacao = await _repository.ObterPorIdAsync(id);
            if (contratacao == null)
                throw new NotFoundException("Contratação não encontrada.");

            await _repository.DeletarAsync(contratacao.Id);
        }
    }
}
