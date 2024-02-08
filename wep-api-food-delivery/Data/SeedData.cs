using Microsoft.AspNetCore.Identity;
using wep_api_food_delivery.Models;

namespace wep_api_food_delivery.Data
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {

            if (context.Users.Any())
            {
                return;
            }

            var users = new[]
            {
                new User { Id = Guid.NewGuid(), Email = "ex1@bk.ru", Password ="user", Role = Enums.Roles.Courier},
                new User { Id = Guid.NewGuid(), Email = "ex2@bk.ru", Password = "user", Role = Enums.Roles.Courier},
                new User { Id = Guid.NewGuid(), Email = "admin@bk.ru", Password = "admin", Role = Enums.Roles.Admin}
            };
            context.Users.AddRange(users);

            context.SaveChanges();
        }
    }
}
