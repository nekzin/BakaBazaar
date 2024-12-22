using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.DataAccess.Repositories;
using BBC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BBC.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private IOrderRepository _orderRepository;
        private ICartRepository _cartRepository;
        private IReviewRepository _reviewRepository;
        private IWishlistRepository _wishlistRepository;

        public UnitOfWork(ApplicationDbContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;

        }
        public ICartRepository Carts => _cartRepository ??= new CartRepository(_context);
        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);
        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        
        public ICategoryRepository CategoryRepository => _categoryRepository;
        // В классе UnitOfWork
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public IReviewRepository Reviews => _reviewRepository ??= new ReviewRepository(_context);
        public IWishlistRepository Wishlist => _wishlistRepository ??= new WishlistRepository(_context);


        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
