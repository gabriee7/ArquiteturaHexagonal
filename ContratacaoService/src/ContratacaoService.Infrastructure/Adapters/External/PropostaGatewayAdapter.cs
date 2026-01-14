using ContratacaoService.Domain.Ports.External.PropostaService;
using ContratacaoService.Domain.Ports.External.PropostaService.DTOs;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace ContratacaoService.Infrastructure.Adapters.External
{
    public class PropostaGatewayAdapter : IPropostaGateway
    {
        private readonly RestClient _client;

        public PropostaGatewayAdapter(IConfiguration configuration)
        {
            var baseUrl = configuration["Services:PropostaApiBaseUrl"];
            _client = new RestClient(baseUrl);
        }

        public async Task<PropostaExternaDto?> ObterPorIdAsync(Guid id)
        {
            var request = new RestRequest($"/api/v1/propostas/{id}");

            var response = await _client.ExecuteAsync<PropostaExternaDto>(request);

            if (!response.IsSuccessful || response.Data == null)
                return null;

            return response.Data;
        }
    }
}
