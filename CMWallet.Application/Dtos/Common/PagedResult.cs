namespace CMWallet.Application.Dtos
{
    public class PagedResult<T>
    {
        public required IReadOnlyList<T> Itens { get; init; }
        public int Pagina { get; init; }
        public int TamanhoPagina { get; init; }
        public int Total { get; init; }
        public int TotalPaginas => TamanhoPagina == 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanhoPagina);
    }
}
