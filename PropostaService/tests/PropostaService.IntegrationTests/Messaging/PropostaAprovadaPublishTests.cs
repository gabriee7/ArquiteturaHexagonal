using FluentAssertions;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports.Messaging.Events;
using PropostaService.Infrastructure.Data;
using PropostaService.IntegrationTests.Configurations;
using System.Net;

namespace PropostaService.IntegrationTests.Messaging
{
    public class PropostaAprovadaPublishTests : IClassFixture<PropostaApiFactory>
    {
        private readonly HttpClient _client;
        private readonly PropostaApiFactory _factory;

        public PropostaAprovadaPublishTests(PropostaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DeveEnviarEventoComDadosCorretosParaOBroker_AoAprovarProposta()
        {
            //Arr
            var nomeSegurado = "Humberto Sergio";
            var valor = 50000m;
            var id = await CriarPropostaNoBanco(nomeSegurado, valor);
            var harness = _factory.Services.GetRequiredService<ITestHarness>();
            //Act
            var response = await _client.PostAsync($"api/v1/propostas/{id}/aprovar", null);
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var enviouMensagem = await harness.Sent.Any<PropostaAprovadaEvent>();
            enviouMensagem.Should().BeTrue("O evento PropostaAprovadaEvent deveria ter sido enviado ao broker.");
            var mensagem = harness.Sent.Select<PropostaAprovadaEvent>().First();
            mensagem.Context.Message.PropostaId.Should().Be(id);
            mensagem.Context.Message.NomeSegurado.Should().Be(nomeSegurado);
            mensagem.Context.Message.Valor.Should().Be(valor);
        }

        private async Task<Guid> CriarPropostaNoBanco(string nome, decimal valor)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var proposta = new Proposta(nome, valor);
            db.Propostas.Add(proposta);
            await db.SaveChangesAsync();
            return proposta.Id;
        }
    }
}