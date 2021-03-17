using OnlineRetailer.CustomerAPI.Core.DomainServices;
using OnlineRetailer.CustomerAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.CustomerAPI.Core.ApplicationServices.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;


		public CustomerService(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}

		public Customer Add(Customer customer)
		{
			return _customerRepository.Add(customer);
		}

		public void Edit(Customer customer)
		{
			_customerRepository.Edit(customer);
		}

		public Customer Get(int id)
		{
			return _customerRepository.Get(id);
		}

		public IEnumerable<Customer> GetAll()
		{
			return _customerRepository.GetAll();
		}

		public void Remove(int id)
		{
			_customerRepository.Remove(id);
		}
	}
}
