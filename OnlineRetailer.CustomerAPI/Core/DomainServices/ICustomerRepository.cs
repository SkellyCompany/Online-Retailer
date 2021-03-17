using OnlineRetailer.CustomerAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.CustomerAPI.Core.DomainServices
{
	public interface ICustomerRepository
	{
		IEnumerable<Customer> GetAll();
		Customer Get(int id);
		Customer Add(Customer customer);
		void Edit(Customer customer);
		void Remove(int id);
	}
}
