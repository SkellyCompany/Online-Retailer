using Microsoft.EntityFrameworkCore;
using OnlineRetailer.Entities;

namespace OnlineRetailer.CustomerAPI.Infrastructure.Database
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options): base(options){}

        public DbSet<Customer> Customers { get; set; }
    }
}
