using wep_api_food.Enums;
using wep_api_food.Models;

namespace wep_api_food.Dtos
{
    public class UserReadModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password {  get; set; }
        public string Token { get; set; }
    }
}
