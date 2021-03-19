using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineRetailer.Entities;

namespace OnlineRetailer.OrderAPI.Infrastructure.Database
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion(new EnumToStringConverter<OrderStatus>());
        }

        public DbSet<Order> Orders { get; set; }
    }
}
