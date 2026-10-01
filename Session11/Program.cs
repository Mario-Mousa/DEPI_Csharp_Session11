using System;
using System.Collections.Generic;
using System.Linq;

namespace Session11
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice;
            do
            {
                Console.WriteLine();
                Console.WriteLine("==== Session 11 ====");
                Console.WriteLine("1 - Section 01: Book delegates");
                Console.WriteLine("2 - Section 02: Parts 1-4 (delegate, Func, Predicate, Action)");
                Console.WriteLine("3 - Section 02: Parts 5-6 (events, subscribe/unsubscribe)");
                Console.WriteLine("4 - Section 02: Part 7 (final flow)");
                Console.WriteLine("5 - Section 02: Bonus (pricing strategies)");
                Console.WriteLine("0 - Exit");
                Console.Write("Enter your choice: ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1: Section01(); break;
                    case 2: Parts1To4(); break;
                    case 3: Parts5And6(); break;
                    case 4: Part7(); break;
                    case 5: Bonus(); break;
                    case 0: Console.WriteLine("Goodbye!"); break;
                    default: Console.WriteLine("Invalid choice."); break;
                }

            } while (choice != 0);
        }

        static List<Book> GetBooks()
        {
            return new List<Book>
            {
                new Book("111", "C# Basics", new[] { "Ahmed Ali" }, new DateTime(2020, 5, 10), 150m),
                new Book("222", "Advanced OOP", new[] { "Sara Adel", "Omar Khaled" }, new DateTime(2022, 1, 20), 200m)
            };
        }

        #region Section 01 - Book delegates
        static void Section01()
        {
            List<Book> books = GetBooks();

            // Q1: Book.ToString() and constructor are implemented in Book.cs
            Console.WriteLine("-- ToString --");
            foreach (Book b in books)
                Console.WriteLine(b);

            // Q2 (a): user-defined delegate
            Console.WriteLine("-- (a) User-defined delegate: GetTitle --");
            LibraryEngine.ProcessBooks(books, BookFunctions.GetTitle);

            // Q2 (b): built-in delegate Func<Book, string>
            Console.WriteLine("-- (b) Built-in Func<Book, string>: GetAuthors --");
            LibraryEngine.ProcessBooksWithFunc(books, BookFunctions.GetAuthors);

            // Q2 (c): Anonymous method (GetISBN)
            Console.WriteLine("-- (c) Anonymous method: GetISBN --");
            LibraryEngine.ProcessBooks(books, delegate (Book B)
            {
                return B.ISBN;
            });

            // Q2 (d): Lambda expression (GetPublicationDate)
            Console.WriteLine("-- (d) Lambda: GetPublicationDate --");
            LibraryEngine.ProcessBooksWithFunc(books, B => B.PublicationDate.ToString("yyyy-MM-dd"));
        }
        #endregion

        #region Parts 1-4
        static void Parts1To4()
        {
            Order order = new Order { Id = 1, CustomerName = "Ahmed", Price = 100m, Quantity = 3 };

            // Part 1: user-defined delegate, different implementations, NO if/else
            Console.WriteLine("-- Part 1 --");
            Console.WriteLine(OrderPricing.CalculateOrderPrice(order, OrderPricing.CalculateTotal));
            Console.WriteLine(OrderPricing.CalculateOrderPrice(order, OrderPricing.CalculateTotalWithDiscount));

            // Part 2: built-in Func<Order, decimal> with lambdas
            Console.WriteLine("-- Part 2 --");
            Console.WriteLine(OrderPricing.CalculateOrderPriceFunc(order, x => x.Price * x.Quantity));
            Console.WriteLine(OrderPricing.CalculateOrderPriceFunc(order, x => x.Price * x.Quantity * 0.9m));
            // Func<> solves: no need to declare a new delegate type for every signature.
            // Func<Order, decimal> already means "takes an Order, returns a decimal",
            // so the same built-in type works everywhere instead of one custom delegate per method shape.

            // Part 3: Predicate<Order> with 3 lambda rules
            Console.WriteLine("-- Part 3 --");
            Console.WriteLine(OrderValidator.ValidateOrder(order, o => o.Quantity > 0));
            Console.WriteLine(OrderValidator.ValidateOrder(order, o => o.Price > 0));
            Console.WriteLine(OrderValidator.ValidateOrder(order, o => !string.IsNullOrWhiteSpace(o.CustomerName)));
            // Predicate<Order> is more expressive than Func<Order, bool> because its NAME says what it is:
            // "a test/condition on an order". Reading the signature tells you the intent
            // (a rule that is either satisfied or not) instead of just "something returning bool".

            // Part 4: same ProcessOrder, different behaviors passed in
            Console.WriteLine("-- Part 4 --");
            Action<Order> printOrder = o => Console.WriteLine($"Order {o.Id} processed.");
            Action<Order> sendConfirmation = o => Console.WriteLine($"Confirmation sent to {o.CustomerName}.");
            Action<Order> writeAudit = o => Console.WriteLine($"[AUDIT] Order {o.Id} at {DateTime.Now:HH:mm:ss}");

            OrderProcessor.ProcessOrder(order, printOrder);
            OrderProcessor.ProcessOrder(order, sendConfirmation);
            OrderProcessor.ProcessOrder(order, writeAudit);

            // Multicast delegate: combine several actions into one and call it once
            Action<Order> all = printOrder + sendConfirmation + writeAudit;
            Console.WriteLine("-- Multicast Action --");
            OrderProcessor.ProcessOrder(order, all);
        }
        #endregion

        #region Parts 5-6
        static void Handler1(Order o) { Console.WriteLine($"Handler1: order {o.Id} processed."); }
        static void Handler2(Order o) { Console.WriteLine($"Handler2: notification sent to {o.CustomerName}."); }
        static void Handler3(Order o) { Console.WriteLine($"Handler3: audit entry for order {o.Id}."); }

        static void Parts5And6()
        {
            Order order = new Order { Id = 2, CustomerName = "Sara", Price = 50m, Quantity = 2 };
            OrderService orderService = new OrderService(x => x.Price * x.Quantity);

            orderService.OrderProcessed += Handler1;
            orderService.OrderProcessed += Handler2;
            orderService.OrderProcessed += Handler3;

            Console.WriteLine("-- All handlers subscribed --");
            orderService.ProcessOrder(order);

            // Part 6: unsubscribe Handler1, it should no longer be called
            orderService.OrderProcessed -= Handler1;

            Console.WriteLine("-- After unsubscribing Handler1 --");
            orderService.ProcessOrder(order);
        }
        #endregion

        #region Part 7 - Final application flow
        static void Part7()
        {
            Order order = new Order { Id = 3, CustomerName = "Omar", Price = 200m, Quantity = 2 };

            // Validate Order (all rules are Predicates)
            List<Predicate<Order>> rules = new List<Predicate<Order>>
            {
                o => o.Quantity > 0,
                o => o.Price > 0,
                o => !string.IsNullOrWhiteSpace(o.CustomerName)
            };

            bool valid = rules.All(rule => OrderValidator.ValidateOrder(order, rule));
            Console.WriteLine("Order valid: " + valid);

            // Calculate Price (Func)
            Func<Order, decimal> pricing = o => o.Price * o.Quantity;
            Console.WriteLine("Total: " + OrderPricing.CalculateOrderPriceFunc(order, pricing));

            // Process Order -> raises OrderProcessed -> Handler1, Handler2, Handler3 run
            OrderService service = new OrderService(pricing);
            service.OrderProcessed += Handler1;
            service.OrderProcessed += Handler2;
            service.OrderProcessed += Handler3;

            if (valid)
                service.ProcessOrder(order);
        }
        #endregion

        #region Bonus - Runtime pricing strategies
        static void Bonus()
        {
            Order order = new Order { Id = 4, CustomerName = "Mona", Price = 100m, Quantity = 2 };

            // The caller decides the strategy; OrderService never knows how price is calculated
            Dictionary<string, Func<Order, decimal>> strategies = new Dictionary<string, Func<Order, decimal>>
            {
                { "Normal Price", o => o.Price * o.Quantity },
                { "10% Discount", o => o.Price * o.Quantity * 0.90m },
                { "20% Discount", o => o.Price * o.Quantity * 0.80m },
                { "VIP Discount", o => o.Price * o.Quantity * 0.70m }
            };

            foreach (KeyValuePair<string, Func<Order, decimal>> strategy in strategies)
            {
                Console.WriteLine("-- " + strategy.Key + " --");
                OrderService service = new OrderService(strategy.Value);
                service.ProcessOrder(order);
            }
        }
        #endregion

        #region Theory Questions (Q1 - Q8)
        // Q1: PriceCalculator vs Func<Order, decimal>
        //   Both represent "takes an Order, returns a decimal". PriceCalculator is a custom delegate type we
        //   had to declare ourselves with a meaningful name. Func<Order, decimal> is a ready-made generic
        //   built-in delegate, so no declaration is needed. They are different types though: a
        //   PriceCalculator variable can't be assigned directly to a Func<Order, decimal> variable.
        //
        // Q2: Action<Order> vs Func<Order, decimal>
        //   Action<Order> takes an Order and returns NOTHING (void): it performs an effect (print, log, send).
        //   Func<Order, decimal> takes an Order and RETURNS a decimal: it computes a value.
        //
        // Q3: Why does Predicate<T> return bool?
        //   Predicate<T> represents a TRUE/FALSE test: "does this item satisfy a condition?".
        //   It is designed for validation, filtering, and searching problems (e.g. List.Find, RemoveAll).
        //
        // Q4: delegate vs event
        //   A delegate is a type that holds references to methods and can be invoked by anyone who has it.
        //   An event is a wrapper around a delegate that restricts access: outside code can only
        //   subscribe (+=) and unsubscribe (-=), and only the declaring class can raise it.
        //
        // Q5: Why can't external code invoke an event declared in another class?
        //   The event keyword exposes only add/remove accessors outside the declaring class. Raising
        //   (Invoke) is allowed only inside the class that declares it. This protects the publisher:
        //   no random code can fake an "order processed" notification.
        //
        // Q6: Multiple handlers on the same event
        //   The event holds a multicast delegate (an invocation list). When raised, ALL subscribed
        //   handlers are called, in the order they were subscribed.
        //
        // Q7: orderService.OrderProcessed += HandleOrderProcessed;
        //   orderService.OrderProcessed : the event on this service instance.
        //   +                           : add to the existing invocation list (not replace it).
        //   =                           : assign the combined list back to the event (+= is shorthand for both).
        //   HandleOrderProcessed        : the method (handler) that will run when the event is raised.
        //
        // Q8: Action<Order> vs event Action<Order>
        //   A public Action<Order> field lets any outside code overwrite it with = (removing all other
        //   subscribers) or invoke it directly. An event Action<Order> uses the same delegate type but
        //   only allows += and -= from outside, and only the owning class can invoke it. So we use an
        //   event to protect the subscriber list and keep control of when notifications are raised.
        #endregion
    }
}