using System;

namespace Session11
{
    // Responsibility: validation only
    public static class OrderValidator
    {
        public static bool ValidateOrder(Order order, Predicate<Order> validationRule)
        {
            return validationRule(order);
        }
    }
}