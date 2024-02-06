using wep_api_food_delivery.Enums;
using wep_api_food_delivery.Models;

namespace wep_api_food_delivery.Dtos
{
    public class ProductReadModel : BaseEntity
    {
        public string Title { get; set; }
        public Categories Category { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
