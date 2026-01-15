using MassTransit;
using PropostaService.Domain.Ports.Messaging;

namespace PropostaService.Infrastructure.Adapters.Messaging
{
    public class MassTransitPublisherAdapter(ISendEndpointProvider sendEndpointProvider) : IMessageBus
    {
        public async Task PublishAsync<T>(T message) where T : class
        {
            await sendEndpointProvider.Send(message);
        }
    }
}
