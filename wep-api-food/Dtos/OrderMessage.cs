﻿using wep_api_food.Enums;
using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class OrderMessage
    {
        public Guid Id { get; set; }
        public ICollection<ProductsInOrder> ProductsInOrder { get; set; }
        public string Address { get; set; }
        public OrderStatuses Status { get; set; }
    }
}
