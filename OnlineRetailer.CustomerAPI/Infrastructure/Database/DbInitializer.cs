using OnlineRetailer.CustomerAPI.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.CustomerAPI.Infrastructure.Database
{
	public class DbInitializer : IDbInitializer
	{
        /// <summary>
        /// Creates and seeds the database.
        /// </summary>
        /// <param name="context">customer context to be used.</param>
        public void Initialize(CustomerContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Customers.Any())
            {
                return;
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer { Name="Greg's Company", Email="greg@gmail.com", Phone="10101010", BillingAddress="Spangsbjerg Kirkevej 101A", ShippingAddress="Spangsbjerg Kirkevej 101A", CreditStanding=8000 },
                new Customer { Name="David's Company", Email="david@yahoo.com", Phone="20202020", BillingAddress="Nørregade 65A", ShippingAddress="Jyllandsgade 62", CreditStanding=1000 },
                new Customer { Name="Máté's Company", Email="mate@hotmail.com", Phone="30303030", BillingAddress="Spangsbjerg Kirkevej 101F", ShippingAddress="Spangsbjerg Kirkevej 101F", CreditStanding=0 },
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
