namespace SalesIndicators.API.DTOs
{
    public class SalesOverTimeDTO
    {
        public string Period { get; set; } = string.Empty; // Ex: "2025-07"
        public decimal TotalSales { get; set; }
    }
}
