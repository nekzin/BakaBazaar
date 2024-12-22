using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.Models;
using Microsoft.EntityFrameworkCore;

namespace BBC.DataAccess.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Set<Order>()
                .Include(o => o.Items) // Загружаем элементы заказа
                .ThenInclude(i => i.Product) // Загружаем данные о продуктах
                .Include(o => o.User) // Загружаем данные о пользователе (если нужно)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Set<Order>().Where(o => o.UserId == userId).ToListAsync();
        }
        public async Task AddOrderAsync(Order order)
        {
            await _context.Set<Order>().AddAsync(order);  // Добавляем новый заказ в контекст
            await _context.SaveChangesAsync();      // Сохраняем изменения в базе данных
        }
        public async Task DeleteAsync(int orderId)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order != null)
            {
                _context.Set<Order>().Remove(order);
            }
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Set<Order>().Update(order);
            await Task.CompletedTask;
        }
        public IQueryable<Order> GetAllQueryable()
        {
            return _context.Set<Order>().AsQueryable();
        }
        public async Task<bool> OrderExistsAsync(int orderId)
        {
            return await _context.Set<Order>().AnyAsync(o => o.OrderId == orderId);
        }
    }

}
