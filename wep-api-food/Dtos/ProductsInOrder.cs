using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class ProductsInOrder
    {
        public ProductReadModel Product { get; set; }
        public int Quantity { get; set; }
    }
}
