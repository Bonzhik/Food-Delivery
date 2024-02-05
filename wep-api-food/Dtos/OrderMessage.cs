using wep_api_food.Enums;
using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class OrderMessage : BaseMessage
    {
        public Guid Id { get; set; }
        public ICollection<ProductsInOrder> ProductsInOrder { get; set; }
        public User User { get; set; }
        public string Status { get; set; }
    }
}
