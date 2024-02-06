using wep_api_food_delivery.Dtos;
using wep_api_food_delivery.Enums;

namespace wep_api_food_delivery.Models
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
