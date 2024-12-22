using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BBC.DataAccess.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task ClearCart(Cart cart)
        {
            cart.Items.Clear();
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<Cart> GetByIdAsync(int cartId)
        {
            return await _context.Carts.Include(c => c.Items)
                                       .FirstOrDefaultAsync(c => c.CartId == cartId);
        }
        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }
        public void RemoveItemFromCart(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);  // Использование DbContext внутри репозитория
        }

        public void DeleteCart(int cartId)
        {
            var cart = _context.Carts.Find(cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
        }
        public void Update(Cart cart)
        {
            _context.Carts.Update(cart);
        }
    }

}
