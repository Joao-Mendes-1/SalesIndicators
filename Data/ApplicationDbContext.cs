using Microsoft.EntityFrameworkCore;
using SalesIndicators.API.Models;

namespace SalesIndicators.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Sale> Sales { get; set; }
    }
}
