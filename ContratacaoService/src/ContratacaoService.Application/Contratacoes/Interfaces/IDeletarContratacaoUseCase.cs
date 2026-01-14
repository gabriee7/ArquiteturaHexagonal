namespace ContratacaoService.Application.Contratacoes.Interfaces
{
    public interface IDeletarContratacaoUseCase
    {
        Task ExecutarAsync(Guid id);
    }
}
