namespace ContratacaoService.Application.Common.DTOs
{
    public class BaseFiltroInput
    {
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
        public string? TermoBusca { get; set; }
        public string Ordenacao { get; set; } = "desc";
    }
}
