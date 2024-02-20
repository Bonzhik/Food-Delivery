using wep_api_food.Models;

namespace wep_api_food.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> Get(Guid id);
        Task<ICollection<T>> GetAll();
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save(T entity);
    }
}
