using BBC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBC.DataAccess.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId);
        Task<Review> GetReviewByProductAndUserIdAsync(int productId, string userId);
    }
}
