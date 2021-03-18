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

		public IEnumerable<Order> GetAll()
		{
			return _orderContext.Orders.AsNoTracking().Include(o => o.OrderLines).ToList();
		}

		public Order Get(int id)
		{
			return _orderContext.Orders.AsNoTracking().Include(o => o.OrderLines).FirstOrDefault(o => o.Id == id);
		}

		public Order Add(Order order)
		{
			Order newOrder = _orderContext.Orders.Add(order).Entity;
			_orderContext.SaveChanges();
			return newOrder;
		}

		public Order Edit(Order order)
		{
			_orderContext.Entry(order).State = EntityState.Modified;
			_orderContext.SaveChanges();
			return order;
		}

		public Order Remove(int id)
		{
			Order order = _orderContext.Orders.FirstOrDefault(o => o.Id == id);
			_orderContext.Orders.Remove(order);
			_orderContext.SaveChanges();
			return order;
		}
	}
}
