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

		public IEnumerable<Order> GetCustomerOrders(int id)
		{
			return _orderRepository.GetCustomerOrders(id);
		}

		public Order Add(Order order)
		{
			if (order == null)
			{
				throw new NullReferenceException("Order is null");
			}
			return _orderRepository.Add(order);
		}

		public Order Edit(Order order)
		{
			if (_orderRepository.Get(order.Id) == null)
			{
				throw new NullReferenceException($"Could not find Order with ID: {order.Id}");
			}
			if (order.Status == OrderStatus.CANCELLED)
			{
				if ((int)order.Status > 1)
				{
					throw new NullReferenceException($"Order cannot be canceled");
				}
			}
			return _orderRepository.Edit(order);
		}

		public Order Remove(int id)
		{
			return _orderRepository.Remove(id);
		}
	}
}
