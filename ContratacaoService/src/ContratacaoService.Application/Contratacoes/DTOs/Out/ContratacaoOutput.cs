using AutoMapper;
using ContratacaoService.Application.Common.DTOs;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Application.Contratacoes.DTOs.Out
{
    [AutoMap(typeof(Contratacao))]
    public class ContratacaoOutput : BaseOutput
    {
        public Guid PropostaId { get; set; }
        public DateTime DataContratacao { get; set; }
    }
}
