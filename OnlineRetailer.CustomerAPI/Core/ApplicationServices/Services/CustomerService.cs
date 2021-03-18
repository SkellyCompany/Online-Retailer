using OnlineRetailer.CustomerAPI.Core.DomainServices;
using OnlineRetailer.Entities;
using System;
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

		public IEnumerable<Customer> GetAll()
		{
			return _customerRepository.GetAll();
		}

		public Customer Get(int id)
		{
			if (_customerRepository.Get(id) == null)
			{
				throw new NullReferenceException($"Could not find Customer with ID: {id}");
			}
			return _customerRepository.Get(id);
		}

		public Customer Add(Customer customer)
		{
			if (customer == null)
			{
				throw new NullReferenceException ("Customer is null");
			}
			return _customerRepository.Add(customer);
		}

		public Customer Edit(Customer customer)
		{
			if (customer == null)
			{
				throw new NullReferenceException("Customer is null");
			}
			if (_customerRepository.Get(customer.Id) == null)
			{
				throw new NullReferenceException($"Could not find Customer with ID: {customer.Id}");
			}
			return _customerRepository.Edit(customer);
		}

		public Customer Remove(int id)
		{
			if (_customerRepository.Get(id) == null)
			{
				throw new NullReferenceException($"Could not find Customer with ID: {id}");
			}
			return _customerRepository.Remove(id);
		}
	}
}
