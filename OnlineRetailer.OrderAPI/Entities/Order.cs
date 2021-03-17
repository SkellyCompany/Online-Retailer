using System;
using System.Collections.Generic;

namespace OnlineRetailer.OrderAPI.Entities
{
    public enum OrderStatus
    {
        PROCESSED = 1,
        IN_TRANSIT = 2,
        DELIVERED = 3,
        CANCELLED = 4
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }
}
