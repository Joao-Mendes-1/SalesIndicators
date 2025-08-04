using System.ComponentModel.DataAnnotations;

namespace SalesIndicators.API.DTOs
{
    public class SalesFilterDto
    {
        // Intervalo de datas (opcional)
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // Filtro por região ou produto (opcional)
        public string? Region { get; set; }
        public string? Product { get; set; }

        // Paginação
        [Range(1, 1000, ErrorMessage = "PageSize deve estar entre 1 e 1000.")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber deve ser maior que zero.")]
        public int PageNumber { get; set; } = 1;
    }
}
