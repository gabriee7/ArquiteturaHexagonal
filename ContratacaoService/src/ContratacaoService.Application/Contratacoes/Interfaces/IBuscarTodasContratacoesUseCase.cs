using ContratacaoService.Application.Common.DTOs;
using ContratacaoService.Application.Contratacoes.DTOs.Out;

namespace ContratacaoService.Application.Contratacoes.Interfaces
{
    public interface IBuscarTodasContratacoesUseCase
    {
        Task<ListaPaginadaOutput<ContratacaoOutput>> ExecutarAsync(BaseFiltroInput input);
    }
}
