using System.Threading.Tasks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Services
{
    /// <summary>
    /// 訂單領域服務 - 處理跨聚合的業務邏輯
    /// 當業務邏輯不適合放在單一實體中時，使用領域服務
    /// </summary>
    public interface IOrderDomainService
    {
        Task<bool> CanProcessOrderAsync(Order order);
        Task<Money> CalculateShippingFeeAsync(Order order, string shippingAddress);
    }

    public class OrderDomainService : IOrderDomainService
    {
        private readonly IInventoryService _inventoryService;
        private readonly IShippingService _shippingService;

        public OrderDomainService(IInventoryService inventoryService, IShippingService shippingService)
        {
            _inventoryService = inventoryService;
            _shippingService = shippingService;
        }

        /// <summary>
        /// 檢查訂單是否可以處理 (庫存檢查)
        /// </summary>
        public async Task<bool> CanProcessOrderAsync(Order order)
        {
            foreach (var item in order.Items)
            {
                var availableStock = await _inventoryService.GetAvailableStockAsync(item.ProductId);
                if (availableStock < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 計算運費 - 跨領域的複雜邏輯
        /// </summary>
        public async Task<Money> CalculateShippingFeeAsync(Order order, string shippingAddress)
        {
            // 免運門檻
            var freeShippingThreshold = new Money(1000, "TWD");
            if (order.TotalAmount.Amount >= freeShippingThreshold.Amount)
            {
                return Money.Zero("TWD");
            }

            // 根據地址計算運費
            return await _shippingService.CalculateFeeAsync(shippingAddress, order.TotalAmount);
        }
    }

    /// <summary>
    /// 庫存服務介面 - 防腐層，避免直接依賴外部系統
    /// </summary>
    public interface IInventoryService
    {
        Task<int> GetAvailableStockAsync(ProductId productId);
        Task ReserveStockAsync(ProductId productId, int quantity);
    }

    /// <summary>
    /// 物流服務介面
    /// </summary>
    public interface IShippingService
    {
        Task<Money> CalculateFeeAsync(string address, Money orderTotal);
    }
}