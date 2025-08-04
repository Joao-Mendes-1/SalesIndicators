using SalesIndicators.API.Data;
using SalesIndicators.API.DTOs;
using SalesIndicators.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SalesIndicators.API.Services
{
    public class SalesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public SalesService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private static MemoryCacheEntryOptions CacheOptions(int minutes, bool sliding = false)
        {
            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Normal
            };

            if (sliding)
                options.SlidingExpiration = TimeSpan.FromMinutes(minutes);
            else
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes);

            return options;
        }

        // Total de vendas (faturamento bruto)
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _cache.GetOrCreateAsync("TotalRevenue", async entry =>
            {
                entry.SetOptions(CacheOptions(5));
                return await _context.Sales.SumAsync(s => s.Quantity * s.UnitPrice);
            });
        }

        // Lucro total
        public async Task<decimal> GetTotalProfitAsync()
        {
            return await _cache.GetOrCreateAsync("TotalProfit", async entry =>
            {
                entry.SetOptions(CacheOptions(5));
                return await _context.Sales.SumAsync(s => s.Quantity * (s.UnitPrice - s.CostPrice));
            });
        }

        // Vendas por região (com DTO)
        public async Task<List<RegionSalesDTO>> GetSalesByRegionAsync()
        {
            var result = await _cache.GetOrCreateAsync("SalesByRegion", async entry =>
            {
                entry.SetOptions(CacheOptions(10));
                return await _context.Sales
                    .GroupBy(s => s.Region)
                    .Select(g => new RegionSalesDTO
                    {
                        Region = g.Key,
                        TotalSales = g.Sum(s => s.Quantity * s.UnitPrice)
                    })
                    .ToListAsync();
            });

            return result ?? new List<RegionSalesDTO>();
        }

        // Produtos mais vendidos (com DTO)
        public async Task<List<ProductSalesDTO>> GetBestSellingProductsAsync(int top = 5)
        {
            string cacheKey = $"BestSellingProducts_Top{top}";
            var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SetOptions(CacheOptions(10));
                return await _context.Sales
                    .GroupBy(s => s.Product)
                    .Select(g => new ProductSalesDTO
                    {
                        Product = g.Key,
                        QuantitySold = g.Sum(s => s.Quantity)
                    })
                    .OrderByDescending(p => p.QuantitySold)
                    .Take(top)
                    .ToListAsync();
            });

            return result ?? new List<ProductSalesDTO>();
        }

        // Margem média
        public async Task<decimal> GetAverageMarginAsync()
        {
            return await _cache.GetOrCreateAsync("AverageMargin", async entry =>
            {
                entry.SetOptions(CacheOptions(5));

                if (!await _context.Sales.AnyAsync())
                    return 0;

                return await _context.Sales
                    .AverageAsync(s => (s.UnitPrice - s.CostPrice) / s.CostPrice);
            });
        }

        // Evolução temporal das vendas (com DTO)
        public async Task<List<SalesOverTimeDTO>> GetSalesOverTimeAsync()
        {
            var result = await _cache.GetOrCreateAsync("SalesOverTime", async entry =>
            {
                entry.SetOptions(CacheOptions(10));

                var grouped = await _context.Sales
                    .GroupBy(s => new { s.Date.Year, s.Date.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalSales = g.Sum(s => s.Quantity * s.UnitPrice)
                    })
                    .ToListAsync();

                return grouped
                    .Select(g => new SalesOverTimeDTO
                    {
                        Period = $"{g.Year}-{g.Month:D2}",
                        TotalSales = g.TotalSales
                    })
                    .OrderBy(dto => dto.Period)
                    .ToList();
            });

            return result ?? new List<SalesOverTimeDTO>();
        }

        // 🔍 Novo método com filtros e paginação
        public async Task<PaginatedResult<Sale>> GetFilteredSalesAsync(SalesFilterDto filter)
        {
            var query = _context.Sales.AsNoTracking().AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(s => s.Date >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(s => s.Date <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.Region))
                query = query.Where(s => s.Region == filter.Region);

            if (!string.IsNullOrWhiteSpace(filter.Product))
                query = query.Where(s => s.Product == filter.Product);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Date)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResult<Sale>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
    }
}
