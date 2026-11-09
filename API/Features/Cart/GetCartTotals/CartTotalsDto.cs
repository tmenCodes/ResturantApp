namespace API.Features.Cart.GetCartTotals;

/// <summary>A cart's subtotal, tax and total, each formatted with two decimals.</summary>
public class CartTotalsDto
{
    public string SubTotal { get; set; } = string.Empty;
    public string TaxAmount { get; set; } = string.Empty;
    public string TotalPrice { get; set; } = string.Empty;
}
