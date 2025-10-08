using System;

namespace ECommerce.Domain.ValueObjects
{
    /// <summary>
    /// 強型別識別碼 - 避免原始型別執著
    /// </summary>
    public record OrderId(Guid Value)
    {
        public static OrderId New() => new OrderId(Guid.NewGuid());
        public static OrderId From(string id) => new OrderId(Guid.Parse(id));
        
        public override string ToString() => Value.ToString();
    }

    public record CustomerId(Guid Value)
    {
        public static CustomerId New() => new CustomerId(Guid.NewGuid());
        public static CustomerId From(string id) => new CustomerId(Guid.Parse(id));
        
        public override string ToString() => Value.ToString();
    }

    public record ProductId(Guid Value)
    {
        public static ProductId New() => new ProductId(Guid.NewGuid());
        public static ProductId From(string id) => new ProductId(Guid.Parse(id));
        
        public override string ToString() => Value.ToString();
    }
}