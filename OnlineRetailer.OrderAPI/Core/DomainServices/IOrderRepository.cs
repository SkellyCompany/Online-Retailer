using OnlineRetailer.OrderAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.OrderAPI.Core.DomainServices
{
	public interface IOrderRepository
	{
		IEnumerable<Order> GetAll();
		Order Get(int id);
		IEnumerable<Order> GetCustomerOrders(int id);
		Order Add(Order order);
		Order Edit(Order order);
		Order Remove(int id);
	}
}
