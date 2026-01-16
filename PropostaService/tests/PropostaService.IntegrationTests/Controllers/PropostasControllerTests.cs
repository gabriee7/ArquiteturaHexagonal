using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Infrastructure.Data;
using PropostaService.IntegrationTests.Configurations;
using System.Net;
using System.Net.Http.Json;

namespace PropostaService.IntegrationTests.Controllers
{
    public class PropostasControllerTests : IClassFixture<PropostaApiFactory>
    {
        private readonly HttpClient _client;
        private readonly PropostaApiFactory _factory;
        private const string BaseUrl = "api/v1/propostas";

        public PropostasControllerTests(PropostaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Deve_CriarProposta_E_Retornar201Created()
        {
            //Arr
            var input = new CriarPropostaInput 
            { 
                NomeSegurado = "Humberto Global",
                Valor = 12000m
            };
            //Act
            var response = await _client.PostAsJsonAsync(BaseUrl, input);
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var output = await response.Content.ReadFromJsonAsync<PropostaOutput>();
            output!.NomeSegurado.Should().Be(input.NomeSegurado);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task Deve_AprovarProposta_QuandoIdForValido()
        {
            //Arr
            var id = await CriarPropostaNoBanco("Ana Julia", 5000m);
            //Act
            var response = await _client.PostAsync($"{BaseUrl}/{id}/aprovar", null);
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var proposta = await db.Propostas.FindAsync(id);
            proposta!.Status.Should().Be(StatusPropostaEnum.Aprovada);
        }

        [Fact]
        public async Task NaoDeve_AtualizarAoTentarComIdsDiferentes()
        {
            //Arr
            var idUrl = Guid.NewGuid();
            var input = new AtualizarPropostaInput 
            { 
                Id = Guid.NewGuid(),
                NomeSegurado = "Inconsistente",
                Valor = 100m
            };
            //Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{idUrl}", input);
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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