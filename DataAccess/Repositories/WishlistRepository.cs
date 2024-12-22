using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBC.DataAccess.Repositories
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Wishlist>> GetWishlistItemsByUserIdAsync(string userId)
        {
            return await _context.Set<Wishlist>()
                .Include(w => w.Product)  // Eagerly load the Product
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<Wishlist> GetWishlistItemByUserAndProductAsync(string userId, int productId)
        {
            return await _context.Set<Wishlist>()
                                 .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }
        public async Task DeleteAsync(Wishlist wishlistItem)
        {
            _context.Set<Wishlist>().Remove(wishlistItem);  // Удаляем запись из базы данных
            await _context.SaveChangesAsync();  // Сохраняем изменения
        }
    }
}
