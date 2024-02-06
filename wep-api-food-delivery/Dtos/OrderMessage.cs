using wep_api_food_delivery.Enums;

namespace wep_api_food_delivery.Dtos
{
    public class OrderMessage
    {
        public Guid Id { get; set; }
        public ICollection<ProductsInOrder> ProductsInOrder { get; set; }
        public string Address { get; set; }
        public OrderStatuses Status { get; set; }
    }
}
