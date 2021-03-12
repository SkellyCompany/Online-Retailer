using Microsoft.EntityFrameworkCore;
using OnlineRetailer.Entities;

namespace OnlineRetailer.CustomerApi.Infrastructure.Database
{
    public class CustomerApiContext : DbContext
    {
        public CustomerApiContext(DbContextOptions<CustomerApiContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
