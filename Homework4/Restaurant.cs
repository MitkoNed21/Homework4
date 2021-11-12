using Homework4;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework4
{
    class Restaurant
    {
        const int minimumAge = 18;

        List<Student> visitors = new();
        Dictionary<Food, int> stock = new();
        Dictionary<Food, int> orderedFoods = new();
        Random random = new();
        bool isOpen = true;
        object lockObj = new();
        decimal profit = 0;

        Semaphore semaphore = new(initialCount: 100, maximumCount: 100);

        public string Name { get; set; }

        public bool IsOpen => isOpen;
        public List<Supplier> Suppliers { get; } = new();

        public Restaurant(string name)
        {
            Name = name;
            OrderedFoods = new ReadOnlyDictionary<Food, int>(orderedFoods);
            Stock = new ReadOnlyDictionary<Food, int>(stock);

            Task.Run(() =>
            {
                Random random = new();
                var number = 100;

                while (number > 2)
                {
                    number = random.Next(0, 101);
                    Thread.Sleep(210);
                }
                
                this.isOpen = false;
                Console.WriteLine("Restaurant has closed!!!");
            });
        }

        /// <summary>
        /// Makes a visitor enter into the restaurant
        /// </summary>
        /// <param name="student">The student who is to enter</param>
        /// <returns>true if the visitor entered successfully
        /// and false if they waited for too long.</returns>
        /// <exception cref="InvalidOperationException">If the student is less than the minimum
        /// allowed for the restaurant or if the restaurant has closed.</exception>
        public bool Enter(Student student)
        {
            var waitingTimeInMs = random.Next(330, 1000);

            bool visitorWaitedTooMuch;
            try
            {
                visitorWaitedTooMuch = !semaphore.WaitOne(waitingTimeInMs);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("The restaurant is closed!");
            }

            lock (visitors)
            {
                if (!this.IsOpen)
                {
                    semaphore.Release();
                    throw new InvalidOperationException("The restaurant is closed!");
                }

                if (visitorWaitedTooMuch)
                {
                    return false;
                }

                // Student can wait in the line until they are told they can't enter
                if (student.Age < minimumAge)
                {
                    semaphore.Release();
                    throw new VisitorNotOldEnoughException(visitor: student, minimumAge);
                }

                visitors.Add(student);
            }

            return true;
        }

        /// <summary>
        /// Makes a visitor leave the restaurant
        /// </summary>
        /// <param name="student">The student who is to leave</param>
        public void Leave(Student student)
        {
            lock (visitors)
            {
                visitors.Remove(student);
            }
            semaphore.Release();
        }

        // TODO: Maybe change to its own collection
        public ReadOnlyCollection<Food> AvailableItems => Stock.Keys.ToList().AsReadOnly();
        public ReadOnlyDictionary<Food, int> OrderedFoods { get; }

        public decimal Profit => profit;

        internal ReadOnlyDictionary<Food, int> Stock { get; }

        public void AddToStock(Food food, int quantity = 1)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException($"Quantity can't be 0 or negative!");
            }

            lock (lockObj)
            {
                if (!this.stock.ContainsKey(food))
                {
                    this.stock[food] = 0;
                }

                this.stock[food] += quantity;
            }
        }

        public bool ServeOrder(Food food, int quantity = 1)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException($"Order quantity can't be 0 or negative!");
            }

            lock (lockObj)
            {
                if (this.stock[food] - quantity < 0)
                {
                    return false;
                }
            
                this.stock[food] -= quantity;

                if (!this.orderedFoods.ContainsKey(food))
                {
                    this.orderedFoods[food] = 0;
                }

                this.orderedFoods[food] += quantity;
                this.profit += food.Price * quantity;
            }

            return true;
        }

        public void ReportAllSales()
        {
            var width = 84;
            var result = new StringBuilder();

            result.AppendLine(new String('=', width));

            const string restaurantSalesString = "Restaurant Sales:";
            result.Append(new String(' ', (width- restaurantSalesString.Length) / 2));
            result.AppendLine($"{restaurantSalesString}");
            
            result.AppendLine(new String('=', width));

            if (this.OrderedFoods.Count == 0)
            {
                result.AppendLine("  The restaurant did not make any sales.");
            }
            else
            {
                result.AppendLine(string.Join(
                    Environment.NewLine,
                    this.OrderedFoods.Select(kvp =>
                        {
                            var quantity = kvp.Value;
                            var food = kvp.Key;

                            return $" - {quantity,3} x {food,-62}{quantity * food.Price,10:F2} lv.";
                        }
                    )
                ));
            }

            result.AppendLine(new String('=', width));

            var restaurantProfitString = $"The restaurant made a profit of {Profit:F2} lv.";
            result.Append(new String(' ', (width - restaurantProfitString.Length) / 2));
            result.AppendLine(restaurantProfitString);

            result.AppendLine(new String('=', width));

            Console.WriteLine(result);
        }

        public void ReportFoodsOutOfStock()
        {
            Console.WriteLine("The restaurant ran out of the following foods:");
            if (this.stock.Any(kvp => kvp.Value == 0))
            {
                Console.WriteLine(string.Join(
                    Environment.NewLine,
                    Stock.Where(kvp => kvp.Value == 0).Select(kvp => $" - {kvp.Key.Name}")
                ));
            }
            else
            {
                Console.WriteLine("  None.");
            }
        }

        ~Restaurant()
        {
            this.semaphore.Dispose();
        }
    }
}
