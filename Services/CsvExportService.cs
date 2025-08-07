using System.Text;
using SalesIndicators.API.DTOs;

namespace SalesIndicators.API.Services
{
    public class CsvExportService
    {
        public byte[] ExportSalesToCsv(List<SaleDto> sales)
        {
            var sb = new StringBuilder();

            // Cabeçalho
            sb.AppendLine("Product,Region,Quantity,UnitPrice,CostPrice,Date");

            // Linhas
            foreach (var sale in sales)
            {
                sb.AppendLine($"{Escape(sale.Product)},{Escape(sale.Region)},{sale.Quantity},{sale.UnitPrice},{sale.CostPrice},{sale.Date:yyyy-MM-dd}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        // Escapar vírgulas, aspas, etc.
        private string Escape(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            if (value.Contains(",") || value.Contains("\""))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
