using BBC.Models;

namespace BBC.DataAccess.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task DeleteAsync(int orderId);  // Метод для удаления заказа
        Task UpdateAsync(Order order);  // Метод для обновления заказа
        Task AddOrderAsync(Order order);
        Task<bool> OrderExistsAsync(int orderId);  // Метод для проверки существования заказа
        IQueryable<Order> GetAllQueryable();
    }
}
