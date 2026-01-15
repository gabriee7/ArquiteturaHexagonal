namespace PropostaService.Domain.Ports.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message) where T : class;
    }
}
