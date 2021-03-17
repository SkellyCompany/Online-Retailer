using OnlineRetailer.OrderAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.OrderAPI.Infrastructure.Database
{
	public class DbInitializer : IDbInitializer
	{
        /// <summary>
        /// Creates and seeds the database.
        /// </summary>
        /// <param name="context">order context to be used.</param>
        public void Initialize(OrderContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Orders.Any())
            {
                return;
            }

            List<Order> orders = new List<Order>
            {
                new Order { Date = DateTime.Today, ProductId = 1, CustomerId = 1, Status = OrderStatus.PROCESSED, Quantity = 2 }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
