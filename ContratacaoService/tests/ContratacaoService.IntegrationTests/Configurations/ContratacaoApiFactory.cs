using ContratacaoService.Infrastructure.Adapters.Messaging;
using ContratacaoService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace ContratacaoService.IntegrationTests.Configurations
{
    public class ContratacaoApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                var dbContextDescriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    .ToList();
                foreach (var descriptor in dbContextDescriptors)
                    services.Remove(descriptor);
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("ContratacaoIntegrationDb");
                });
                var mtDescriptors = services
                    .Where(d => d.ServiceType.Namespace?.Contains("MassTransit") == true)
                    .ToList();
                foreach (var d in mtDescriptors)
                    services.Remove(d);
                services.AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<PropostaAprovadaConsumer>();
                    x.UsingInMemory((busContext, cfg) =>
                    {
                        cfg.UseRawJsonSerializer();

                        cfg.ConfigureEndpoints(busContext);
                    });
                });
                services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    });
            });
            builder.UseEnvironment("Testing");
        }
    }
}