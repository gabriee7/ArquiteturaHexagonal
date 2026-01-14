using ContratacaoService.Domain.Ports.External.PropostaService.Enums;
using System.Text.Json.Serialization;

namespace ContratacaoService.Domain.Ports.External.PropostaService.DTOs
{
    public class PropostaExternaDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusPropostaEnum Status { get; set; }
        public decimal Valor { get; set; }
    }
}
