using System;

namespace Session11
{
    // Responsibility: run ANY behavior after an order is processed.
    // The behavior comes from the caller, so this method never changes.
    public static class OrderProcessor
    {
        public static void ProcessOrder(Order order, Action<Order> action)
        {
            action(order);
        }
    }
}