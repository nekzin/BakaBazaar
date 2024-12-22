using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBC.DataAccess.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Set<Review>().Where(r => r.ProductId == productId).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId)
        {
            return await _context.Set<Review>().Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Review> GetReviewByProductAndUserIdAsync(int productId, string userId)
        {
            return await _context.Set<Review>()
                         .Where(r => r.ProductId == productId && r.UserId == userId)
                         .FirstOrDefaultAsync();
        }
    }
}
