using BBC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBC.DataAccess.Interfaces
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetWishlistItemsByUserIdAsync(string userId);
        Task<Wishlist> GetWishlistItemByUserAndProductAsync(string userId, int productId);
        Task DeleteAsync(Wishlist wishlistItem);

    }
}
