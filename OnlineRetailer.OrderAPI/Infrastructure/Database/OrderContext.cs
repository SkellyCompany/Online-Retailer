using Microsoft.EntityFrameworkCore;
using OnlineRetailer.OrderAPI.Entities;

namespace OnlineRetailer.OrderAPI.Infrastructure.Database
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
