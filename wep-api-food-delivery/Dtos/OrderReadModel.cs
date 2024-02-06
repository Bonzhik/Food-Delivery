using wep_api_food_delivery.Models;

namespace wep_api_food_delivery.Dtos
{
    public class OrderReadModel : BaseEntity
    {
        public string Status { get; set; }
        public string Address {  get; set; }
        public ICollection<OrderReadItemModel> Items { get; set; }
    }
}
