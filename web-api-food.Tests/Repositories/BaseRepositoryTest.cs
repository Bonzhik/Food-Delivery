using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using wep_api_food.Data;
using wep_api_food.Models;
using wep_api_food.Repositories.Implementations;

namespace web_api_food.Repositories
{
    public class BaseRepositoryTests 
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BaseRepository<Product>> _logger;

        public BaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _logger = A.Fake<ILogger<BaseRepository<Product>>>();
        }

        [Fact]
        public async Task Create_Should_Add_Entity_To_Context()
        {
            // Arrange
            var repository = new BaseRepository<Product>(_context, _logger);
            var entity = new Product() { Title = "product", Category = wep_api_food.Enums.Categories.Drink};

            // Act
            await repository.Create(entity);

            // Assert
            _context.Set<Product>().Should().NotBeEmpty();
        }

        [Fact]
        public async Task Get_Should_Return_Entity_By_Id()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entity = new Product { Id = entityId, Title = "Product", Category = wep_api_food.Enums.Categories.Drink }; 
            await _context.Set<Product>().AddAsync(entity);
            await _context.SaveChangesAsync();

            var repository = new BaseRepository<Product>(_context, _logger);

            // Act
            var result = await repository.Get(entityId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(entity);
        }
    }
}
