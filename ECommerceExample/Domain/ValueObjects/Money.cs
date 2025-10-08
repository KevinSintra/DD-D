using System;

namespace ECommerce.Domain.ValueObjects
{
    /// <summary>
    /// 金額值物件 - 封裝金額和幣別
    /// 值物件特性：不可變、無身分識別、可比較
    /// </summary>
    public class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("金額不能為負數", nameof(amount));
            
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("幣別不能為空", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        // 值物件的相等性比較
        public override bool Equals(object obj)
        {
            if (obj is not Money other) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        // 業務操作
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("不能加總不同幣別的金額");
            
            return new Money(Amount + other.Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Amount:F2} {Currency}";
        }

        public static Money Zero(string currency) => new Money(0, currency);
    }
}