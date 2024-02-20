using Microsoft.EntityFrameworkCore;
using wep_api_food.Data;
using wep_api_food.Models;
using wep_api_food.Repositories.Interfaces;

namespace wep_api_food.Repositories.Implementations
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger _logger;

        public BaseRepository(ApplicationDbContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Create(T entity)
        {
            _logger.LogInformation(
                   "Попытка добавления в базу сущности {@EntityType}, {@EntityId}, {@DateTimeNow}",
                   typeof(T).Name,
                   entity.Id,
                   DateTime.UtcNow);
            try
            {
                await _context.Set<T>().AddAsync(entity);
                return await Save(entity);
            } catch (Exception ex)
            {
                _logger.LogError(
                    "Ошибка добавления в базу сущности {@EntityType}, {@EntityId}, {@DateTimeNow}",
                    typeof(T).Name,
                    entity.Id,
                    DateTime.UtcNow);
                return false;
            }
        }

        public virtual async Task<bool> Delete(T entity)
        {
            _logger.LogInformation("Попытка удаления из базы сущности {@EntityType}, {@EntityId}, {@DateTimeNow}",
                   typeof(T).Name,
                   entity.Id,
                   DateTime.UtcNow);
            try
            {
                _context.Remove(entity);
                return await Save(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Ошибка удаления из базы сущности {@EntityType}, {@EntityId}, {@DateTimeNow}",
                    typeof(T).Name,
                    entity.Id,
                    DateTime.UtcNow);
                return false;
            }
        }

        public async Task<T> Get(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<ICollection<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<bool> Save(T entity)
        {
            try
            {
                var save = await _context.SaveChangesAsync();
                _logger.LogInformation(
                    "Данные о сущности {@EntityType}, {@EntityId}, {@DateTimeNow} успешно сохранены",
                    typeof(T).Name,
                    entity.Id,
                    DateTime.UtcNow);
                return save > 0 ? true : false;
            } catch (Exception ex)
            {
                _logger.LogError(
                    "Ошибка сохранения в базу сущности {@EntityType}, {@EntityId}, {@DateTimeNow} ",
                    typeof(T).Name,
                    entity.Id,
                    DateTime.UtcNow);
                return false;
            }
        }

        public async Task<bool> Update(T entity)
        {
            _logger.LogInformation(
                   "Попытка обновления в базе сущности {@EntityType}, {@EntityId}, {@DateTimeNow}",
                   typeof(T).Name,
                   entity.Id,
                   DateTime.UtcNow);
            try
            {
                _context.Update(entity);
                return await Save(entity);
            } catch (Exception ex)
            {
                _logger.LogError(
                    "Ошибка обновления в базе сущности {@EntityType}, {@EntityId}, {@DateTimeNow} ",
                    typeof(T).Name,
                    entity.Id,
                    DateTime.UtcNow);
                return false;
            }
        }
    }
}
