using wep_api_food_delivery.Data;
using wep_api_food_delivery.Enums;

namespace wep_api_food_delivery.Services.Implementations
{
    public class EventProcessor
    {
        public EventProcessor()
        {
            
        }
        public void HandleEvent(OrderStatuses status)
        {
            switch (status)
            {
                case OrderStatuses.Create:
                    break;
                case OrderStatuses.Cancel:
                    break;
            }
        }
        private bool OnCreate()
        {
            return false;
        }
        private bool OnDelete()
        {
            return false;
        }
    }
}
