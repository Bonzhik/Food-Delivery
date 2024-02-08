using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class OrderCreateModel
    {
        public List<ProductsInOrder> ProductsInOrder { get; set; }
        public string Address { get; set; }
    }
}
