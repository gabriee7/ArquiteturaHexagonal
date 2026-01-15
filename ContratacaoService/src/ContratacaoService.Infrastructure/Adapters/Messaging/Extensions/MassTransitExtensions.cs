using ContratacaoService.Infrastructure.Adapters.Messaging.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ContratacaoService.Infrastructure.Adapters.Messaging.Extensions
{
    public static class MassTransitExtensions
    {
        public static void AddMassTransitWithAutoDiscovery(
            this IServiceCollection services,
            IConfiguration configuration,
            params Assembly[] consumerAssemblies)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMq");

            services.AddMassTransit(x =>
            {
                x.AddConsumers(consumerAssemblies);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings["Host"], rabbitMqSettings["VirtualHost"], h =>
                    {
                        h.Username(rabbitMqSettings["Username"]);
                        h.Password(rabbitMqSettings["Password"]);
                    });

                    cfg.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
                    ConfigureConsumersFromAssemblies(context, cfg, consumerAssemblies);
                });
            });
        }

        private static void ConfigureConsumersFromAssemblies(
            IBusRegistrationContext context,
            IRabbitMqBusFactoryConfigurator cfg,
            Assembly[] assemblies)
        {
            var consumerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
                .ToList();

            var consumersByQueue = consumerTypes
                .Select(type => new
                {
                    Type = type,
                    Metadata = type.GetCustomAttribute<ConsumerConfigurationAttribute>()
                })
                .Where(x => x.Metadata != null)
                .GroupBy(x => x.Metadata!.QueueName);

            foreach (var queueGroup in consumersByQueue)
            {
                var queueName = queueGroup.Key;
                var consumers = queueGroup.ToList();

                cfg.ReceiveEndpoint(queueName, e =>
                {
                    e.ConfigureConsumeTopology = false;

                    foreach (var consumer in consumers)
                    {
                        ConfigureConsumer(context, e, consumer.Type);
                    }
                });
            }
        }

        private static void ConfigureConsumer(
            IBusRegistrationContext context,
            IRabbitMqReceiveEndpointConfigurator endpoint,
            Type consumerType)
        {
            var configureConsumerMethod = typeof(MassTransitExtensions)
                .GetMethod(nameof(ConfigureConsumerGeneric), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(consumerType);

            configureConsumerMethod?.Invoke(null, new object[] { endpoint, context });
        }

        private static void ConfigureConsumerGeneric<TConsumer>(
            IRabbitMqReceiveEndpointConfigurator endpoint,
            IBusRegistrationContext context)
            where TConsumer : class, IConsumer
        {
            endpoint.ConfigureConsumer<TConsumer>(context);
        }
    }
}
