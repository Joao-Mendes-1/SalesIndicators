namespace SalesIndicators.API.DTOs
{
    public class SaleDto
    {
        public string Product { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
