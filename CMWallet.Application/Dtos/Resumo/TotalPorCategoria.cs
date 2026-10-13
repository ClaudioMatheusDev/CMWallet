using CMWallet.Domain.Enums;

namespace CMWallet.Application.Dtos
{
    /// <summary>Agregado de transações por categoria, tipo e situação de pagamento, calculado no banco.</summary>
    public sealed record TotalPorCategoria(
        int CategoriaId,
        string Nome,
        Tipo Tipo,
        bool Pago,
        decimal Total,
        int Quantidade);
}
