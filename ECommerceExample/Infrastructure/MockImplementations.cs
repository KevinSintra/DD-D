using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Services;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Examples
{
    /// <summary>
    /// 記憶體內的訂單儲存庫實作 - 用於測試和範例
    /// </summary>
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<OrderId, Order> _orders = new();

        public Task<Order> GetByIdAsync(OrderId id)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order);
        }

        public Task<Order> GetByIdWithItemsAsync(OrderId id)
        {
            // 在實際的 ORM 實作中，這裡會包含相關實體
            return GetByIdAsync(id);
        }

        public Task SaveAsync(Order order)
        {
            _orders[order.Id] = order;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(OrderId id)
        {
            _orders.Remove(id);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(OrderId id)
        {
            return Task.FromResult(_orders.ContainsKey(id));
        }

        public Task<Order[]> GetOrdersByCustomerAsync(CustomerId customerId)
        {
            var orders = _orders.Values
                .Where(o => o.CustomerId.Equals(customerId))
                .ToArray();
            return Task.FromResult(orders);
        }
    }

    /// <summary>
    /// 模擬庫存服務
    /// </summary>
    public class MockInventoryService : IInventoryService
    {
        public Task<int> GetAvailableStockAsync(ProductId productId)
        {
            // 模擬：所有商品都有充足庫存
            return Task.FromResult(100);
        }

        public Task ReserveStockAsync(ProductId productId, int quantity)
        {
            // 模擬預留庫存
            Console.WriteLine($"🔒 預留庫存 - 商品:{productId}, 數量:{quantity}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 模擬物流服務
    /// </summary>
    public class MockShippingService : IShippingService
    {
        public Task<Money> CalculateFeeAsync(string address, Money orderTotal)
        {
            // 簡單的運費計算邏輯
            if (orderTotal.Amount >= 1000)
                return Task.FromResult(Money.Zero("TWD"));
            
            return Task.FromResult(new Money(100, "TWD"));
        }
    }
}