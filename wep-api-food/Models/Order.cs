using wep_api_food.Enums;

namespace wep_api_food.Models
{
    public class Order : BaseEntity
    {
        public User User { get; set; }
        public OrderStatuses Status { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
