using System;

namespace Session11
{
    // Responsibility: process an order and notify subscribers.
    // It does NOT know how the price is calculated: it receives that behavior as a delegate (Bonus).
    public class OrderService
    {
        private readonly Func<Order, decimal> pricingStrategy;

        public event Action<Order> OrderProcessed;

        public OrderService(Func<Order, decimal> pricingStrategy)
        {
            this.pricingStrategy = pricingStrategy;
        }

        public void ProcessOrder(Order order)
        {
            decimal total = pricingStrategy(order);
            Console.WriteLine($"Order {order.Id} total price: {total}");

            // Raise the event only if there is at least one subscriber
            OrderProcessed?.Invoke(order);
        }
    }
}