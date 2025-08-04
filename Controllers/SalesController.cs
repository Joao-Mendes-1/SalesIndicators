using Microsoft.AspNetCore.Mvc;
using SalesIndicators.API.Services;
using SalesIndicators.API.DTOs;

namespace SalesIndicators.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly SalesService _salesService;

        public SalesController(SalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var result = await _salesService.GetTotalRevenueAsync();
            return Ok(result);
        }

        [HttpGet("total-profit")]
        public async Task<IActionResult> GetTotalProfit()
        {
            var result = await _salesService.GetTotalProfitAsync();
            return Ok(result);
        }

        [HttpGet("by-region")]
        public async Task<IActionResult> GetSalesByRegion()
        {
            var result = await _salesService.GetSalesByRegionAsync();
            return Ok(result);
        }

        [HttpGet("best-selling-products")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery] int top = 5)
        {
            var result = await _salesService.GetBestSellingProductsAsync(top);
            return Ok(result);
        }

        [HttpGet("average-margin")]
        public async Task<IActionResult> GetAverageMargin()
        {
            var result = await _salesService.GetAverageMarginAsync();
            return Ok(result);
        }

        [HttpGet("sales-over-time")]
        public async Task<IActionResult> GetSalesOverTime()
        {
            var result = await _salesService.GetSalesOverTimeAsync();
            return Ok(result);
        }

        // NOVO ENDPOINT DE FILTRO COM PAGINAÇÃO
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredSales([FromQuery] SalesFilterDto filter)
        {
            var result = await _salesService.GetFilteredSalesAsync(filter);
            return Ok(result);
        }

    }
}
