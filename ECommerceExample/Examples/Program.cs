using System;
using System.Threading.Tasks;
using ECommerce.Application.Services;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Examples
{
    /// <summary>
    /// 使用範例 - 展示如何使用 DDD 設計的系統
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // 注意：在實際應用中，這些依賴應該透過 DI 容器注入
            var orderService = new OrderApplicationService(
                new InMemoryOrderRepository(), 
                new OrderDomainService(new MockInventoryService(), new MockShippingService()));

            // 使用案例：建立並處理訂單
            await CreateAndProcessOrderExample(orderService);
        }

        static async Task CreateAndProcessOrderExample(OrderApplicationService orderService)
        {
            try
            {
                Console.WriteLine("=== DDD 電商訂單系統範例 ===\n");

                // 1. 建立訂單
                var customerId = CustomerId.New();
                var orderId = await orderService.CreateOrderAsync(customerId);
                Console.WriteLine($"✓ 建立訂單：{orderId}");

                // 2. 新增商品到訂單
                var productId1 = ProductId.New();
                var productId2 = ProductId.New();
                
                await orderService.AddItemToOrderAsync(orderId, productId1, "MacBook Pro", 45000, 1);
                await orderService.AddItemToOrderAsync(orderId, productId2, "Magic Mouse", 2500, 2);
                Console.WriteLine("✓ 新增商品到訂單");

                // 3. 查看訂單詳情
                var orderDetails = await orderService.GetOrderDetailsAsync(orderId);
                Console.WriteLine($"\n📋 訂單詳情：");
                Console.WriteLine($"   訂單ID: {orderDetails.Id}");
                Console.WriteLine($"   狀態: {orderDetails.Status}");
                Console.WriteLine($"   總金額: {orderDetails.TotalAmount:F2} {orderDetails.Currency}");
                Console.WriteLine($"   商品項目:");
                
                foreach (var item in orderDetails.Items)
                {
                    Console.WriteLine($"     - {item.ProductName}: {item.Quantity} x {item.UnitPrice:F2} = {item.TotalPrice:F2}");
                }

                // 4. 確認訂單
                await orderService.ConfirmOrderAsync(orderId);
                Console.WriteLine("\n✓ 訂單已確認");

                // 5. 查看更新後的狀態
                orderDetails = await orderService.GetOrderDetailsAsync(orderId);
                Console.WriteLine($"📋 更新後狀態: {orderDetails.Status}");

                Console.WriteLine("\n=== 範例完成 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 錯誤: {ex.Message}");
            }
        }
    }
}