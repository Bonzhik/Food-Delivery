﻿using wep_api_food_delivery.Enums;

namespace wep_api_food_delivery.Dtos
{
    public class OrderNotify
    {
        public Guid Id {  get; set; }
        public OrderStatuses Status {  get; set; }
    }
}
