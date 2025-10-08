using System;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// 訂單項目實體 - 訂單聚合的一部分
    /// </summary>
    public class OrderItem
    {
        public ProductId ProductId { get; private set; }
        public string ProductName { get; private set; }
        public Money UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        
        // 計算屬性
        public Money Price => new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);

        public OrderItem(ProductId productId, string productName, Money unitPrice, int quantity)
        {
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            
            if (quantity <= 0)
                throw new ArgumentException("數量必須大於零", nameof(quantity));
            
            Quantity = quantity;
        }

        // 重新建構 (用於 ORM)
        private OrderItem() { }

        public void ChangeQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("數量必須大於零", nameof(newQuantity));
            
            Quantity = newQuantity;
        }
    }
}