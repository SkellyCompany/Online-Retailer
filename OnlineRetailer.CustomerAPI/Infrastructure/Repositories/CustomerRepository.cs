using Microsoft.EntityFrameworkCore;
using OnlineRetailer.CustomerAPI.Core.DomainServices;
using OnlineRetailer.CustomerAPI.Infrastructure.Database;
using OnlineRetailer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.CustomerAPI.Infrastructure.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly CustomerContext _customerContext;


		public CustomerRepository(CustomerContext context)
		{
			_customerContext = context;
		}

		public IEnumerable<Customer> GetAll()
		{
			return _customerContext.Customers.AsNoTracking().ToList();
		}

		public Customer Get(int id)
		{
			return _customerContext.Customers.AsNoTracking().FirstOrDefault(c => c.Id == id);
		}

		public Customer Add(Customer customer)
		{
			Customer newCustomer = _customerContext.Customers.Add(customer).Entity;
			_customerContext.SaveChanges();
			return newCustomer;
		}

		public Customer Edit(Customer customer)
		{
			_customerContext.Attach(customer).State = EntityState.Modified;
			_customerContext.SaveChanges();
			return customer;
		}

		public Customer Remove(int id)
		{
			Customer customer = _customerContext.Customers.FirstOrDefault(c => c.Id == id);
			_customerContext.Customers.Remove(customer);
			_customerContext.SaveChanges();
			return customer;
		}
	}
}
