namespace wep_api_food_delivery.Models
{
    public class OrderItem : BaseEntity
    {
        public string Title {  get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
    }
}
