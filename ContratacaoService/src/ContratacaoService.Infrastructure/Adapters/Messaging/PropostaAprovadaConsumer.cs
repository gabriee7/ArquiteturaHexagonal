using ContratacaoService.Application.Contratacoes.DTOs.In;
using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Infrastructure.Adapters.Messaging.Attributes;
using ContratacaoService.Infrastructure.Adapters.Messaging.Events;
using MassTransit;

namespace ContratacaoService.Infrastructure.Adapters.Messaging
{
    [ConsumerConfiguration("contratacao-proposta-aprovada")]
    public class PropostaAprovadaConsumer : IConsumer<PropostaAprovadaEvent>
    {
        private readonly ICriarContratacaoUseCase _useCase;

        public PropostaAprovadaConsumer(ICriarContratacaoUseCase useCase)
        {
            _useCase = useCase;
        }

        public async Task Consume(ConsumeContext<PropostaAprovadaEvent> context)
        {
            var input = new CriarContratacaoInput
            {
                PropostaId = context.Message.PropostaId
            };

            await _useCase.ExecutarAsync(input);
        }
    }
}
