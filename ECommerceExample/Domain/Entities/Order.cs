using System;
using System.Collections.Generic;
using System.Linq;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// 訂單實體 - 聚合根
    /// 實體特性：有唯一身分識別、可變、封裝業務邏輯
    /// </summary>
    public class Order
    {
        // 私有集合，對外只提供只讀存取
        private readonly List<OrderItem> _items = new();

        public OrderId Id { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        
        // 只讀集合，防止外部修改
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        
        // 計算屬性 - 業務邏輯
        public Money TotalAmount 
        { 
            get 
            {
                if (!_items.Any()) 
                    return Money.Zero("TWD");
                
                var total = _items.First().Price;
                foreach (var item in _items.Skip(1))
                {
                    total = total.Add(item.Price);
                }
                return total;
            }
        }

        // 建構子 - 確保物件建立時的有效狀態
        public Order(OrderId id, CustomerId customerId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Draft;
        }

        // 重新建構 (用於從儲存庫載入)
        private Order() { }

        // 業務方法 - 封裝業務規則
        public void AddItem(ProductId productId, string productName, Money unitPrice, int quantity)
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("只有草稿狀態的訂單才能新增商品");
            
            if (quantity <= 0)
                throw new ArgumentException("數量必須大於零", nameof(quantity));

            // 檢查是否已存在相同商品
            var existingItem = _items.FirstOrDefault(x => x.ProductId.Equals(productId));
            if (existingItem != null)
            {
                existingItem.ChangeQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var item = new OrderItem(productId, productName, unitPrice, quantity);
                _items.Add(item);
            }
        }

        public void RemoveItem(ProductId productId)
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("只有草稿狀態的訂單才能移除商品");

            var item = _items.FirstOrDefault(x => x.ProductId.Equals(productId));
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("只有草稿狀態的訂單才能確認");
            
            if (!_items.Any())
                throw new InvalidOperationException("訂單必須包含至少一個商品才能確認");

            Status = OrderStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("已出貨或已送達的訂單無法取消");

            Status = OrderStatus.Cancelled;
        }

        public void Ship()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException("只有已確認的訂單才能出貨");

            Status = OrderStatus.Shipped;
        }
    }

    /// <summary>
    /// 訂單狀態枚舉
    /// </summary>
    public enum OrderStatus
    {
        Draft,      // 草稿
        Confirmed,  // 已確認
        Shipped,    // 已出貨
        Delivered,  // 已送達
        Cancelled   // 已取消
    }
}