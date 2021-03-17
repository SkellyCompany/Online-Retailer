﻿using OnlineRetailer.OrderAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.OrderAPI.Core.ApplicationServices
{
	public interface IOrderService
	{
		IEnumerable<Order> GetAll();
		Order Get(int id);
		Order Add(Order order);
		void Edit(Order order);
		void Remove(int id);
	}
}