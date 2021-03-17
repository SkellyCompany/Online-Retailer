using Microsoft.EntityFrameworkCore;
using OnlineRetailer.CustomerAPI.Core.DomainServices;
using OnlineRetailer.CustomerAPI.Entities;
using OnlineRetailer.CustomerAPI.Infrastructure.Database;
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

		public Customer Add(Customer customer)
		{
			Customer newCustomer = _customerContext.Customers.Add(customer).Entity;
			_customerContext.SaveChanges();
			return newCustomer;
		}

		public void Edit(Customer customer)
		{
			_customerContext.Entry(customer).State = EntityState.Modified;
			_customerContext.SaveChanges();
		}

		public Customer Get(int id)
		{
			return _customerContext.Customers.FirstOrDefault(c => c.Id == id);
		}

		public IEnumerable<Customer> GetAll()
		{
			return _customerContext.Customers.ToList();
		}

		public void Remove(int id)
		{
			Customer customer = _customerContext.Customers.FirstOrDefault(c => c.Id == id);
			_customerContext.Customers.Remove(customer);
			_customerContext.SaveChanges();
		}
	}
}
