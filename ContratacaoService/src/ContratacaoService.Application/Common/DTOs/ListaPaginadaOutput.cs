namespace ContratacaoService.Application.Common.DTOs
{
    public class ListaPaginadaOutput<T>
    {
        public List<T> Itens { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalItens { get; set; }

        public ListaPaginadaOutput() { }

        public ListaPaginadaOutput(List<T> itens, int count, int paginaAtual, int tamanhoPagina)
        {
            Itens = itens;
            TotalItens = count;
            PaginaAtual = paginaAtual;
            TotalPaginas = (int)Math.Ceiling(count / (double)tamanhoPagina);
        }
    }
}
