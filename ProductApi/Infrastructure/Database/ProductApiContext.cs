using Microsoft.EntityFrameworkCore;
using OnlineRetailer.Entities;

namespace OnlineRetailer.ProductApi.Infrastructure.Database
{
    public class ProductApiContext : DbContext
    {
        public ProductApiContext(DbContextOptions<ProductApiContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
