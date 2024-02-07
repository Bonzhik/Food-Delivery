using wep_api_food.Enums;

namespace wep_api_food.Dtos
{
    public class OrderNotify
    {
        public Guid Id {  get; set; }
        public OrderStatuses Status {  get; set; }
    }
}
