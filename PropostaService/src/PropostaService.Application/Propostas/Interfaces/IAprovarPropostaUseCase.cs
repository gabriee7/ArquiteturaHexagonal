namespace PropostaService.Application.Propostas.Interfaces
{
    public interface IAprovarPropostaUseCase
    {
        Task ExecutarAsync(Guid id);
    }
}
