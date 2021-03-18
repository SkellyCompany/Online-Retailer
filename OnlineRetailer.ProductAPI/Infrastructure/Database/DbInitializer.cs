using OnlineRetailer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.ProductAPI.Infrastructure.Database
{
    public class DbInitializer : IDbInitializer
    {
        /// <summary>
        /// Creates and seeds the database.
        /// </summary>
        /// <param name="context">product context to be used.</param>
        public void Initialize(ProductContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return;
            }

            List<Product> products = new List<Product>
            {
                new Product { Name = "Hammer", Price = 100, ItemsInStock = 10, ItemsReserved = 0 },
                new Product { Name = "Screwdriver", Price = 70, ItemsInStock = 20, ItemsReserved = 0 },
                new Product { Name = "Drill", Price = 500, ItemsInStock = 2, ItemsReserved = 0 }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
