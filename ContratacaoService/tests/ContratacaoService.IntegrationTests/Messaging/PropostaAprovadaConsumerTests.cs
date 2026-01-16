using ContratacaoService.Infrastructure.Adapters.Messaging.Events;
using ContratacaoService.Infrastructure.Data;
using ContratacaoService.IntegrationTests.Configurations;
using ContratacaoService.Domain.Ports.External.PropostaService;
using ContratacaoService.Domain.Ports.External.PropostaService.DTOs;
using ContratacaoService.Domain.Ports.External.PropostaService.Enums;
using ContratacaoService.Domain.Entities;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace ContratacaoService.IntegrationTests.Messaging
{
    public class PropostaAprovadaConsumerTests : IClassFixture<ContratacaoApiFactory>
    {
        private readonly ContratacaoApiFactory _factory;

        public PropostaAprovadaConsumerTests(ContratacaoApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task DeveCriarContratacaoNoBancoComSucesso_QuandoReceberEventoPropostaAprovada()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var evento = new PropostaAprovadaEvent { PropostaId = propostaId };
            var propostaGatewayMock = new Mock<IPropostaGateway>();
            propostaGatewayMock
                .Setup(x => x.ObterPorIdAsync(propostaId))
                .ReturnsAsync(new PropostaExternaDto
                {
                    Id = propostaId,
                    Status = StatusPropostaEnum.Aprovada,
                    Valor = 79000m,
                    CreatedAt = DateTime.Now
                });
            var factoryComMock = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPropostaGateway));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddScoped(_ => propostaGatewayMock.Object);
                });
            });
            var harness = factoryComMock.Services.GetRequiredService<ITestHarness>();
            //Act
            await harness.Bus.Publish(evento);
            //Ass
            var consumiu = await harness.Consumed.Any<PropostaAprovadaEvent>();
            if (await harness.Published.Any<Fault<PropostaAprovadaEvent>>())
            {
                var fault = await harness.Published.SelectAsync<Fault<PropostaAprovadaEvent>>().First();
                var msg = fault.Context.Message.Exceptions.FirstOrDefault()?.Message;
                msg.Should().BeNull($"O Consumer falhou com erro: {msg}");
            }
            consumiu.Should().BeTrue("A mensagem deve ser consumida pelo MassTransit.");
            var contratacao = await AguardarContratacaoNoBanco(propostaId, factoryComMock);
            contratacao.Should().NotBeNull("O registro de contratação deve ser salvo no banco.");
            contratacao!.PropostaId.Should().Be(propostaId);
        }

        [Fact]
        public async Task NaoDeveCriarContratacao_QuandoPropostaNaoForEncontradaNoGateway()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var evento = new PropostaAprovadaEvent { PropostaId = propostaId };
            var propostaGatewayMock = new Mock<IPropostaGateway>();
            propostaGatewayMock
                .Setup(x => x.ObterPorIdAsync(propostaId))
                .ReturnsAsync((PropostaExternaDto?)null);
            var factoryComMock = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPropostaGateway));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddScoped(_ => propostaGatewayMock.Object);
                });
            });
            var harness = factoryComMock.Services.GetRequiredService<ITestHarness>();
            //Act
            await harness.Bus.Publish(evento);
            //Ass
            await harness.Consumed.Any<PropostaAprovadaEvent>();
            var houveErro = await harness.Published.Any<Fault<PropostaAprovadaEvent>>();
            houveErro.Should().BeTrue("Deve ocorrer um Fault pois a proposta não existe.");

            var contratacao = await AguardarContratacaoNoBanco(propostaId, factoryComMock, tentativas: 2);
            contratacao.Should().BeNull("Não deve haver registro no banco para proposta inexistente.");
        }

        [Fact]
        public async Task NaoDeveCriarContratacao_QuandoStatusDaPropostaNaoForAprovada()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var evento = new PropostaAprovadaEvent { PropostaId = propostaId };
            var propostaGatewayMock = new Mock<IPropostaGateway>();
            propostaGatewayMock
                .Setup(x => x.ObterPorIdAsync(propostaId))
                .ReturnsAsync(new PropostaExternaDto
                {
                    Id = propostaId,
                    Status = StatusPropostaEnum.Rejeitada,
                    Valor = 79000m,
                    CreatedAt = DateTime.Now
                });
            var factoryComMock = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPropostaGateway));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddScoped(_ => propostaGatewayMock.Object);
                });
            });
            var harness = factoryComMock.Services.GetRequiredService<ITestHarness>();
            //Act
            await harness.Bus.Publish(evento);
            //Ass
            await harness.Consumed.Any<PropostaAprovadaEvent>();
            var contratacao = await AguardarContratacaoNoBanco(propostaId, factoryComMock, tentativas: 2);
            contratacao.Should().BeNull("Não deve criar contratação para propostas rejeitadas.");
        }

        private async Task<Contratacao?> AguardarContratacaoNoBanco(
            Guid propostaId, 
            WebApplicationFactory<Program> factory, 
            int tentativas = 5)
        {
            for (int i = 0; i < tentativas; i++)
            {
                using var scope = factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var contratacao = await db.Contratacoes
                    .FirstOrDefaultAsync(c => c.PropostaId == propostaId);
                if (contratacao != null) return contratacao;
                await Task.Delay(500);
            }
            return null;
        }
    }
}