using wep_api_food.Enums;

namespace wep_api_food.Models
{
    public class Product : BaseEntity
    {
        public string Title { get; set; }
        public Categories Category { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
