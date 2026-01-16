using AutoMapper;
using PropostaService.Application.Common.DTOs;
using PropostaService.Domain.Entities;

namespace PropostaService.Application.Propostas.DTOs.Out
{
    [AutoMap(typeof(Proposta))]
    public class PropostaOutput : BaseOutput
    {
        public string NomeSegurado { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
