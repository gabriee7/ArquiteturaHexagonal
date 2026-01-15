using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Domain.Ports.Messaging.Events;

namespace PropostaService.Infrastructure.Adapters.Messaging.Config
{
    public static class MassTransitBrokerConfigurator
    {
        public static void AddMassTransitProducer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMq");

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings["Host"], rabbitMqSettings["VirtualHost"], h =>
                    {
                        h.Username(rabbitMqSettings["Username"]);
                        h.Password(rabbitMqSettings["Password"]);
                    });
                    cfg.UseRawJsonSerializer();
                    ConfigureMessageRoutes(rabbitMqSettings);
                });
            });
        }

        private static void ConfigureMessageRoutes(IConfigurationSection settings)
        {
            MapRoute<PropostaAprovadaEvent>(settings["QueueNameContratacaoAprovada"]);
        }

        private static void MapRoute<T>(string? queueName) where T : class
        {
            if (!string.IsNullOrWhiteSpace(queueName))
                EndpointConvention.Map<T>(new Uri($"queue:{queueName}"));
        }
    }
}