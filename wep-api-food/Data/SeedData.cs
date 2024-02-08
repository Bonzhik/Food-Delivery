using wep_api_food.Helpers.Hash;
using wep_api_food.Models;

namespace wep_api_food.Data
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            var _passwordHasher = new PasswordHasher();

            if (context.AnyDataExists())
            {
                return;
            }

            var users = new[] 
            {
                new User { Id = Guid.NewGuid(), Email = "ex1@bk.ru", Password = _passwordHasher.HashPassword("user"), Role = Enums.Roles.User},
                new User { Id = Guid.NewGuid(), Email = "ex2@bk.ru", Password = _passwordHasher.HashPassword("user"), Role = Enums.Roles.User},
                new User { Id = Guid.NewGuid(), Email = "admin@bk.ru", Password = _passwordHasher.HashPassword("admin"), Role = Enums.Roles.Admin}
            };
            context.Users.AddRange(users);

            var products = new[]
            {
                new Product { Id = Guid.NewGuid(), Title = "title1", Category = Enums.Categories.Sushi, Price = 1000, Quantity =1000 },
                new Product { Id = Guid.NewGuid(), Title = "title2", Category = Enums.Categories.Pizza, Price = 500, Quantity =1000 },
                new Product { Id = Guid.NewGuid(), Title = "title3", Category = Enums.Categories.Drink, Price = 250, Quantity =1000 },
            };
            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }
}
