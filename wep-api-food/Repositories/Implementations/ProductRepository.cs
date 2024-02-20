using wep_api_food.Data;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;

namespace wep_api_food.Repositories.Implementations
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger) : base(context, logger)
        {
        }
        public async Task<bool> IsExists(string title)
        {
            return _context.Products.Any(p => p.Title == title);
        }
    }
}
