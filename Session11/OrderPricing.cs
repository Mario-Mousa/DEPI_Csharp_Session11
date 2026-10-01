using System;

namespace Session11
{
    // Responsibility: pricing logic only
    public static class OrderPricing
    {
        // Methods that match the PriceCalculator delegate
        public static decimal CalculateTotal(Order order)
        {
            return order.Price * order.Quantity;
        }

        public static decimal CalculateTotalWithDiscount(Order order)
        {
            decimal discount = 10m; // fixed discount amount
            return order.Price * order.Quantity - discount;
        }

        // Part 1: uses the user-defined delegate
        public static decimal CalculateOrderPrice(Order order, PriceCalculator calculator)
        {
            return calculator(order);
        }

        // Part 2: uses the built-in Func<> (different name to avoid overload ambiguity with lambdas)
        public static decimal CalculateOrderPriceFunc(Order order, Func<Order, decimal> calculator)
        {
            return calculator(order);
        }
    }
}