using wep_api_food_delivery.Enums;

namespace wep_api_food_delivery.Models
{
    public class Order : BaseEntity
    {
        public ICollection<OrderItem> OrderItems { get; set; }
        public OrderStatuses Status { get; set; }
        public User User { get; set; }
        public string Address { get; set; }
    }
}
