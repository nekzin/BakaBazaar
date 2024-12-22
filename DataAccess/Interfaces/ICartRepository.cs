using BBC.Models;

namespace BBC.DataAccess.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> GetByIdAsync(int cartId);
        Task AddCartAsync(Cart cart);
        Task ClearCart(Cart cart);
        void DeleteCart(int cartId);
        void Update(Cart cart);

        void RemoveItemFromCart(CartItem cartItem);
    }

}
