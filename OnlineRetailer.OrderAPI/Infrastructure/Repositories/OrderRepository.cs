using Microsoft.EntityFrameworkCore;
using OnlineRetailer.OrderAPI.Core.DomainServices;
using OnlineRetailer.OrderAPI.Entities;
using OnlineRetailer.OrderAPI.Infrastructure.Database;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.OrderAPI.Infrastructure.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly OrderContext _orderContext;


		public OrderRepository(OrderContext context)
		{
			_orderContext = context;
		}

		public Order Add(Order order)
		{
			Order newOrder = _orderContext.Orders.Add(order).Entity;
			_orderContext.SaveChanges();
			return newOrder;
		}

		public void Edit(Order order)
		{
			_orderContext.Entry(order).State = EntityState.Modified;
			_orderContext.SaveChanges();
		}

		public Order Get(int id)
		{
			return _orderContext.Orders.FirstOrDefault(o => o.Id == id);
		}

		public IEnumerable<Order> GetAll()
		{
			return _orderContext.Orders.ToList();
		}

		public void Remove(int id)
		{
			Order order = _orderContext.Orders.FirstOrDefault(o => o.Id == id);
			_orderContext.Orders.Remove(order);
			_orderContext.SaveChanges();
		}
	}
}
