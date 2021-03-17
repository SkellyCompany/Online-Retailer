using OnlineRetailer.OrderAPI.Core.DomainServices;
using OnlineRetailer.OrderAPI.Entities;
using System;
using System.Collections.Generic;

namespace OnlineRetailer.OrderAPI.Core.ApplicationServices.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;


		public OrderService(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}
		public Order Add(Order order)
		{
			return _orderRepository.Add(order);
		}

		public void Edit(Order order)
		{
			_orderRepository.Edit(order);
		}

		public Order Get(int id)
		{
			return _orderRepository.Get(id);
		}

		public IEnumerable<Order> GetAll()
		{
			return _orderRepository.GetAll();
		}

		public void Remove(int id)
		{
			_orderRepository.Remove(id);
		}
	}
}
