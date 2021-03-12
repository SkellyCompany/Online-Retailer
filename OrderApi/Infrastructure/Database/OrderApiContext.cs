using Microsoft.EntityFrameworkCore;
using OnlineRetailer.Entities;

namespace OnlineRetailer.OrderApi.Infrastructure.Database
{
    public class OrderApiContext : DbContext
    {
        public OrderApiContext(DbContextOptions<OrderApiContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
