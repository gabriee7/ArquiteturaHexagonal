using System.Net;
using System.Net.Http.Json;
using ContratacaoService.Application.Common.DTOs;
using ContratacaoService.Application.Contratacoes.DTOs.In;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Domain.Ports.External.PropostaService;
using ContratacaoService.Domain.Ports.External.PropostaService.DTOs;
using ContratacaoService.Domain.Ports.External.PropostaService.Enums;
using ContratacaoService.IntegrationTests.Configurations;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ContratacaoService.IntegrationTests.Controllers
{
    public class ContratacoesControllerTests : IClassFixture<ContratacaoApiFactory>
    {
        private readonly ContratacaoApiFactory _factory;
        private const string BaseUrl = "api/v1/contratacoes";

        public ContratacoesControllerTests(ContratacaoApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task DeveCriarContratacao_QuandoInputForValido()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var input = new CriarContratacaoInput { PropostaId = propostaId };
            var gatewayMock = CriarMockGateway(propostaId);
            var client = CriarClientComMock(gatewayMock);
            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, input);
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ContratacaoOutput>();
            result.Should().NotBeNull();
            result!.PropostaId.Should().Be(propostaId);
        }

        [Fact]
        public async Task DeveRetornarListaPaginada_QuandoBuscarTodas()
        {
            //Arr
            var client = _factory.CreateClient();
            //Act
            var response = await client.GetAsync(BaseUrl);
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ListaPaginadaOutput<ContratacaoOutput>>();
            result.Should().NotBeNull();
            result!.Itens.Should().NotBeNull();
        }

        [Fact]
        public async Task DeveRetornarNoContent_QuandoDeletarContratacaoExistente()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var gatewayMock = CriarMockGateway(propostaId);
            var client = CriarClientComMock(gatewayMock);
            var createResponse = await client.PostAsJsonAsync(BaseUrl, new CriarContratacaoInput { PropostaId = propostaId });
            var contratacao = await createResponse.Content.ReadFromJsonAsync<ContratacaoOutput>();
            //Act
            var response = await client.DeleteAsync($"{BaseUrl}/{contratacao!.Id}");
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeveRetornarContratacao_QuandoBuscarPorPropostaId()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var gatewayMock = CriarMockGateway(propostaId);
            var client = CriarClientComMock(gatewayMock);
            await client.PostAsJsonAsync(BaseUrl, new CriarContratacaoInput { PropostaId = propostaId });
            //Act
            var response = await client.GetAsync($"{BaseUrl}/proposta/{propostaId}");
            //Ass
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ContratacaoOutput>();
            result!.PropostaId.Should().Be(propostaId);
        }

        private Mock<IPropostaGateway> CriarMockGateway(Guid propostaId)
        {
            var mock = new Mock<IPropostaGateway>();
            mock.Setup(x => x.ObterPorIdAsync(propostaId))
                .ReturnsAsync(new PropostaExternaDto
                {
                    Id = propostaId,
                    Status = StatusPropostaEnum.Aprovada,
                    Valor = 1000m,
                    CreatedAt = DateTime.UtcNow
                });
            return mock;
        }

        private HttpClient CriarClientComMock(Mock<IPropostaGateway> mock)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPropostaGateway));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddScoped(_ => mock.Object);
                });
            }).CreateClient();
        }
    }
}