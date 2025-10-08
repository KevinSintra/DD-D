using System.Threading.Tasks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Repositories
{
    /// <summary>
    /// 訂單儲存庫介面 - 定義在領域層，實作在基礎設施層
    /// 提供聚合的持久化抽象
    /// </summary>
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(OrderId id);
        Task<Order> GetByIdWithItemsAsync(OrderId id);
        Task SaveAsync(Order order);
        Task DeleteAsync(OrderId id);
        
        // 查詢方法
        Task<bool> ExistsAsync(OrderId id);
        Task<Order[]> GetOrdersByCustomerAsync(CustomerId customerId);
    }
}