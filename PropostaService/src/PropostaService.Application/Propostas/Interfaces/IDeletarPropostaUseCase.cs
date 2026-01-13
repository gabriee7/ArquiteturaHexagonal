namespace PropostaService.Application.Propostas.Interfaces
{
    public interface IDeletarPropostaUseCase
    {
        Task<bool> ExecutarAsync(Guid id);
    }
}
