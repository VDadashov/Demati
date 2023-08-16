﻿namespace Demati.Models
{
    public class OrderItem:BaseEntity
    {
        public Order? Order { get; set; }
        public int? OrderId { get; set; }
        public Product? Product { get; set; }
        public int? ProductId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public double? Shipping { get; set; }
    }
}
