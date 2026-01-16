using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PropostaService.Domain.Ports.Messaging.Events;
using PropostaService.Infrastructure.Data;

namespace PropostaService.IntegrationTests.Configurations
{
    public class PropostaApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                var dbContextDescriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    .ToList();
                foreach (var descriptor in dbContextDescriptors) services.Remove(descriptor);
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestsDb");
                });
                var descriptors = services.Where(d => d.ServiceType.Namespace?.Contains("MassTransit") == true).ToList();
                foreach (var d in descriptors) services.Remove(d);
                services.AddMassTransitTestHarness(x =>
                {
                    x.UsingInMemory((busContext, cfg) =>
                    {
                        var rabbitMqSettings = context.Configuration.GetSection("RabbitMq");
                        var queueName = rabbitMqSettings["QueueNameContratacaoAprovada"];
                        if (!string.IsNullOrWhiteSpace(queueName))
                            EndpointConvention.Map<PropostaAprovadaEvent>(new Uri($"queue:{queueName}"));
                        cfg.ConfigureEndpoints(busContext);
                    });
                });
            });

            builder.UseEnvironment("Testing");
        }
    }
}