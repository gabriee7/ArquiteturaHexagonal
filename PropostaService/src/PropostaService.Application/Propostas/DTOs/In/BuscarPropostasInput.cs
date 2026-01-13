using PropostaService.Application.Common.DTOs;
using PropostaService.Domain.Enums;

namespace PropostaService.Application.Propostas.DTOs.In
{
    public class BuscarPropostasInput : BaseFiltroInput
    {
        public StatusPropostaEnum? Status { get; set; }
    }
}
