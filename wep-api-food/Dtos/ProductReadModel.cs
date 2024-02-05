using wep_api_food.Enums;
using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class ProductReadModel : BaseEntity
    {
        public string Title { get; set; }
        public Categories Category { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
