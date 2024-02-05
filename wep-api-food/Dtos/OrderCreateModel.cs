using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class OrderCreateModel
    {
        public ICollection<ProductsInOrder> ProductsInOrder { get; set; }
    }
}
