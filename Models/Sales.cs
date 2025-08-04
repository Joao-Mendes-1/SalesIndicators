namespace SalesIndicators.API.Models;

public class Sale
{
    public int Id { get; set; }
    public string Product { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public DateTime Date { get; set; }
}
