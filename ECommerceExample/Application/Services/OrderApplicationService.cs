using System;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Services;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// 訂單應用服務 - 協調領域物件執行業務用例
    /// 應用服務不包含業務邏輯，只是編排領域物件的互動
    /// </summary>
    public class OrderApplicationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDomainService _orderDomainService;

        public OrderApplicationService(
            IOrderRepository orderRepository, 
            IOrderDomainService orderDomainService)
        {
            _orderRepository = orderRepository;
            _orderDomainService = orderDomainService;
        }

        /// <summary>
        /// 建立新訂單用例
        /// </summary>
        public async Task<OrderId> CreateOrderAsync(CustomerId customerId)
        {
            var orderId = OrderId.New();
            var order = new Order(orderId, customerId);
            
            await _orderRepository.SaveAsync(order);
            
            return orderId;
        }

        /// <summary>
        /// 新增商品到訂單用例
        /// </summary>
        public async Task AddItemToOrderAsync(OrderId orderId, ProductId productId, 
            string productName, decimal unitPrice, int quantity)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException($"找不到訂單 {orderId}");

            var money = new Money(unitPrice, "TWD");
            order.AddItem(productId, productName, money, quantity);
            
            await _orderRepository.SaveAsync(order);
        }

        /// <summary>
        /// 確認訂單用例 - 包含業務規則檢查
        /// </summary>
        public async Task ConfirmOrderAsync(OrderId orderId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
                throw new InvalidOperationException($"找不到訂單 {orderId}");

            // 使用領域服務檢查業務規則
            var canProcess = await _orderDomainService.CanProcessOrderAsync(order);
            if (!canProcess)
                throw new InvalidOperationException("訂單無法處理，請檢查商品庫存");

            // 執行領域邏輯
            order.Confirm();
            
            await _orderRepository.SaveAsync(order);
        }

        /// <summary>
        /// 取消訂單用例
        /// </summary>
        public async Task CancelOrderAsync(OrderId orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException($"找不到訂單 {orderId}");

            order.Cancel();
            
            await _orderRepository.SaveAsync(order);
        }

        /// <summary>
        /// 查詢訂單詳情
        /// </summary>
        public async Task<OrderDto> GetOrderDetailsAsync(OrderId orderId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id.ToString(),
                CustomerId = order.CustomerId.ToString(),
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount.Amount,
                Currency = order.TotalAmount.Currency,
                Items = order.Items.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId.ToString(),
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice.Amount,
                    Quantity = x.Quantity,
                    TotalPrice = x.Price.Amount
                }).ToArray()
            };
        }
    }

    /// <summary>
    /// 訂單資料傳輸物件 - 用於應用層與外部層的資料交換
    /// </summary>
    public class OrderDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public OrderItemDto[] Items { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}