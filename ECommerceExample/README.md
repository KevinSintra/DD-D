# 電商訂單系統 - DDD 範例

這是一個簡單的 Domain-Driven Design 範例，展示電商系統中的訂單管理功能。

## 領域概念

- **訂單 (Order)**: 客戶的購買請求
- **訂單項目 (OrderItem)**: 訂單中的商品項目
- **客戶 (Customer)**: 購買者
- **商品 (Product)**: 可販售的商品

## 專案結構

```
Domain/
├── Entities/        # 實體
├── ValueObjects/    # 值物件
├── Services/        # 領域服務
└── Repositories/    # 儲存庫介面
```