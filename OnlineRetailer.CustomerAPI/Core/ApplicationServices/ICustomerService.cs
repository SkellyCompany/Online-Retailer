using OnlineRetailer.CustomerAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.CustomerAPI.Core.ApplicationServices
{
	public interface ICustomerService
	{
		IEnumerable<Customer> GetAll();
		Customer Get(int id);
		Customer Add(Customer customer);
		void Edit(Customer customer);
		void Remove(int id);
	}
}
