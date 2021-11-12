using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Homework4
{
    class Program
    {
        static Random random = new();
        private static Restaurant restaurant = new("Restaurant");

        static void Main(string[] args)
        {
            FillRestaurantInitially(restaurant);

            var studentThreads = new List<Thread>();
            var suppliersThreads = new List<Thread>();

            for (int i = 0; i < 300; i++)
            {
                var studentAge = random.Next(14, 27);
                var studentBudget = random.Next(10, 100);

                char genderLetter;
                if (random.Next(1, 101) < 51)
                {
                    genderLetter = 'M';
                }
                else
                {
                    genderLetter = 'F';
                }

                var studentName = GenerateRandomName(genderLetter);

                var Student = new Student(studentName, studentAge, studentBudget)
                {
                    Restaurant = restaurant
                };

                var t = new Thread(Student.DoSomething);
                t.Start();
                studentThreads.Add(t);
            }

            var suppliersCount = random.Next(2, 6);
            for (int i = 0; i < suppliersCount; i++)
            {
                var supplier = new Supplier(GenerateRandomName(genderLetter: 'M'));

                restaurant.Suppliers.Add(supplier);
                suppliersThreads.Add(new Thread(SupplierWorker));
            }

            for (int i = 0; i < restaurant.Suppliers.Count; i++)
            {
                suppliersThreads[i].Start(restaurant.Suppliers[i]);
            }

            foreach (var t in studentThreads)
            {
                t.Join();
            }

            foreach (var t in suppliersThreads)
            {
                t.Join();
            }
            
            Console.WriteLine();
            restaurant.ReportAllSales();
            restaurant.ReportFoodsOutOfStock();
        }

        private static void FillRestaurantInitially(Restaurant restaurant)
        {
            restaurant.AddToStock(new("French Fries", 3.50m, discount: 0.30m));
            restaurant.AddToStock(new("Hamburger", 6.50m));
            restaurant.AddToStock(new("Cheeseburger", 7.70m, discount: 1.00m));
            restaurant.AddToStock(new("Shopska Salad", 4.60m));
            restaurant.AddToStock(new("Ceaser Salad", 4.35m));
            restaurant.AddToStock(new("Margarita Pizza 16cm", 6.00m));
            restaurant.AddToStock(new("Margarita Pizza 20cm", 7.20m, discount: 0.50m));
            restaurant.AddToStock(new("Margarita Pizza 24cm", 8.00m));
            restaurant.AddToStock(new("Running Chicken Pizza 16cm", 6.30m));
            restaurant.AddToStock(new("Running Chicken Pizza 20cm", 7.50m));
            restaurant.AddToStock(new("Running Chicken Pizza 24cm", 8.20m, discount: 1.00m));
            restaurant.AddToStock(new("Maccaroni with Cheese", 4.45m));
            restaurant.AddToStock(new("Lasagna", 5.80m));
            restaurant.AddToStock(new("Carbonara Pasta", 6.40m));
            restaurant.AddToStock(new("Chicken Wings", 5.35m));
            restaurant.AddToStock(new("Fried Cheese", 3.30m, discount: 0.55m));
            restaurant.AddToStock(new("Meatball", 1.00m));
            restaurant.AddToStock(new("Bread slice", 0.20m));
            restaurant.AddToStock(new("Cheesecake Slice", 2.40m, discount: 0.45m));
            restaurant.AddToStock(new("Pancake with chocolate", 2.20m));
            restaurant.AddToStock(new("Baked Potatoes", 4.50m));
            restaurant.AddToStock(new("Icecream Ball", 1.20m));
            restaurant.AddToStock(new("Vegetable Soup", 5.30m, discount: 0.70m));

            foreach (var food in restaurant.AvailableItems)
            {
                restaurant.AddToStock(food, random.Next(7, 12));
            }
        }

        private static string[] firstNamesM = new[]
        {
            "Marin", "Yulian", "Tihomir", "Panayot", "Yordan", "Zdravko", "Zhivko",
            "Viktor", "Vasil", "Martin", "Ivaylo","Mitko", "Dimitar", "Daniel",
            "Valentin", "Slav", "Iliyan", "Ivan", "Kaloyan", "Stoyan",
            "Petar", "Georgi", "Konstantin", "Mihail", "Radomir", "Nikolay"
        };

        private static string[] firstNamesF = new[]
        {
            "Marina", "Yuliana", "Tihomira", "Yordanka", "Zdravka", "Zhivka",
            "Viktoria", "Vasilka", "Martina", "Ivayla", "Mitka", "Dimitrina",
            "Daniela", "Valentina", "Slavka", "Iliyana", "Ivana", "Kaloyana", "Stoyana",
            "Gergana", "Konstantina", "Mihaila", "Radomira", "Nikolina"
        };

        private static string[] lastNamesM = new[]
        {
            "Marinov", "Yulianov", "Tihomirov", "Panayotov", "Yordanov", "Zdravkov",
            "Zhivkov", "Vasilev", "Martinov", "Ivaylov", "Mitev", "Dimitrov",
            "Danielov","Valentinov", "Slavov", "Iliyanov", "Ivanov", "Kaloyanov",
            "Stoyanov", "Petrov", "Georgiev", "Konstantinov", "Mihailov",
            "Radomirov", "Nikolaev"
        };

        private static string[] lastNamesF = new[]
        {
            "Marinova", "Yulianova", "Tihomirova", "Panayotova", "Yordanova",
            "Zdravkova", "Zhivkova", "Vasileva", "Martinova", "Ivaylova", "Miteva",
            "Dimitrov", "Danielova", "Valentinova", "Slavova", "Iliyanova",
            "Ivanova", "Kaloyanova", "Stoyanova", "Petrova", "Georgieva",
            "Konstantinova", "Mihailova", "Radomirova", "Nikolaeva"
        };
        

        private static string GenerateRandomName(char genderLetter)
        {
            var r = new Random();

            string lastName;
            string firstName;

            if (char.ToLower(genderLetter) == 'm')
            {
                firstName = firstNamesM[r.Next(0, firstNamesM.Length)];
                lastName = lastNamesM[r.Next(0, lastNamesM.Length)];
            }
            else
            {
                firstName = firstNamesF[r.Next(0, firstNamesF.Length)];
                lastName = lastNamesF[r.Next(0, lastNamesF.Length)];
            }

            return $"{firstName} {lastName}";
        }

        
        static public void SupplierWorker(object supplierObj)
        {
            var supplier = (Supplier)supplierObj;

            while (restaurant.IsOpen)
            {
                var suppliesInCurrentDelivery = new List<Food>();

                // Supplier sends 7 different items each when supplying
                for (int j = 0; j < 7; j++)
                {
                    var quantity = random.Next(3, 8);

                    foreach (var productInfo in restaurant.Stock)
                    {
                        // should probably also in lock // TODO: Look through code
                        var restaurantQuantity = productInfo.Value;
                        if (restaurantQuantity < 6)
                        {
                            var food = productInfo.Key;
                            if (!suppliesInCurrentDelivery.Contains(food))
                            {
                                suppliesInCurrentDelivery.Add(food);
                                supplier.SupplyRestaurant(food, quantity, restaurant);
                            }
                            break;
                        }
                    }
                }

                // simulate time passing
                var minTime = 400;
                var maxTime = 800;
                Thread.Sleep(random.Next(minTime, maxTime));
            }
        }
    }
}
