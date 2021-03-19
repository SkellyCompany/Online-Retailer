﻿using System;
namespace OnlineRetailer.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public int Quantity { get; set; }
    }

    public enum OrderStatus
    {
        PROCESSED, IN_TRANSIT, DELIVERED
    }
}