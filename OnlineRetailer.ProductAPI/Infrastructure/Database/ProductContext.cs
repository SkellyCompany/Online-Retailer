using Microsoft.EntityFrameworkCore;
using OnlineRetailer.ProductAPI.Entities;

namespace OnlineRetailer.ProductAPI.Infrastructure.Database
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
