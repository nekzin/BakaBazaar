using BBC.Models;

namespace BBC.DataAccess.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        void Update(Category category);
        void Remove(Category category);
        Task<bool> ExistsAsync(int id);
    }

}
