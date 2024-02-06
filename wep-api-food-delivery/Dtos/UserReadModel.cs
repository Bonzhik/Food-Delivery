using wep_api_food_delivery.Models;

namespace wep_api_food_delivery.Dtos
{
    public class UserReadModel : BaseEntity
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
