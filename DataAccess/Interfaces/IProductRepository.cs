using BBC.Models;

namespace BBC.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        void Update(Product product);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Product> GetProductWithReviewsAsync(int id);
    }
}
