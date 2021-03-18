using OnlineRetailer.OrderAPI.Core.DomainServices;
using OnlineRetailer.OrderAPI.Entities;
using System;
using System.Collections.Generic;

namespace OnlineRetailer.OrderAPI.Core.ApplicationServices.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;


		public IEnumerable<Order> GetAll()
		{
			return _orderRepository.GetAll();
		}

		public Order Get(int id)
		{
			if (_orderRepository.Get(id) == null)
			{
				throw new NullReferenceException($"Could not find Order with ID: {id}");
			}
			return _orderRepository.Get(id);
		}

		public OrderService(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}
		public Order Add(Order order)
		{
			return _orderRepository.Add(order);
		}

		public Order Edit(Order order)
		{
			return _orderRepository.Edit(order);
		}

		public Order Remove(int id)
		{
			return _orderRepository.Remove(id);
		}
	}
}
