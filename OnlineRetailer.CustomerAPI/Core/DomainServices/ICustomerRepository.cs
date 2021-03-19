using OnlineRetailer.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.CustomerAPI.Core.DomainServices
{
	public interface ICustomerRepository
	{
		IEnumerable<Customer> GetAll();
		Customer Get(int id);
		Customer Add(Customer customer);
		Customer Edit(Customer customer);
		Customer Remove(int id);
	}
}
