using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.Models;
using Microsoft.EntityFrameworkCore;

namespace BBC.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToListAsync();
        }
        public async Task<Product> GetProductWithReviewsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)  // Загружаем связанные с отзывами данные о пользователе
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Images) // Загружаем связанные изображения
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }


        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == id);
        }
    }
}
