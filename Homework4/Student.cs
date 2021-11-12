using Homework4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework4
{
    class Student
    {
        enum StudentActivities { Walk, VisitRestaurant, GoHome }

        enum RestaurantActivities { Eat, Talk, Leave }

        Random random = new Random();
        private bool inRestaurant = false;
        private decimal ballance = 0;
        private bool learntCannotEnter = false;

        public string Name { get; set; }
        public int Age { get; }
        public decimal Ballance => ballance;
        
        public Restaurant Restaurant { get; set; }
        public bool InRestaurant => inRestaurant;

        private StudentActivities GetRandomActivity()
        {
            var n = random.Next(10);
            if (learntCannotEnter)
            {
                if (n < 4) return StudentActivities.Walk;
                return StudentActivities.GoHome;
            }

            if (n < 3) return StudentActivities.Walk;
            if (n < 8) return StudentActivities.VisitRestaurant;
            return StudentActivities.GoHome;
        }
        private RestaurantActivities GetRandomRestaurantActivity()
        {
            var n = random.Next(10);
            if (this.Ballance > 1.50m)
            {
                if (n < 4) return RestaurantActivities.Eat;
                if (n < 9) return RestaurantActivities.Talk;
                return RestaurantActivities.Leave;
            }

            if (n < 3) return RestaurantActivities.Talk;
            return RestaurantActivities.Leave;
        }

        private void WalkOut()
        {
            Console.WriteLine($"{Name} is walking in the streets.");
            Thread.Sleep(100);
        }

        private void VisitRestaurant()
        {
            Console.WriteLine($"{Name} is getting in the line to enter.");

            try
            {
                inRestaurant = Restaurant.Enter(this);

                if (!inRestaurant)
                {
                    Console.WriteLine($"{Name} left the queue. They waited too much.");
                }
                else
                {
                    Console.WriteLine($"{Name} entered.");
                }
            }
            catch (VisitorNotOldEnoughException e)
            {
                Console.WriteLine(e.Message);
                inRestaurant = false;

                learntCannotEnter = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Name} could not enter the restaurant: {e.Message}");
                inRestaurant = false;

                if (!this.Restaurant.IsOpen)
                {
                    learntCannotEnter = true;
                }
            }

            while (inRestaurant)
            {
                // Make everyone leave when restaurant closes
                if (!this.Restaurant.IsOpen)
                {
                    lock (this.Restaurant)
                    {
                        LeaveRestaurant();
                    }
                    learntCannotEnter = true;

                    break;
                }

                RestaurantActivities nextActivity;
                if (this.Restaurant.IsOpen)
                {
                    nextActivity = GetRandomRestaurantActivity();
                }
                else
                {
                    nextActivity = RestaurantActivities.Leave;
                }

                switch (nextActivity)
                {
                    case RestaurantActivities.Eat:
                        var foodToOrder = Restaurant.AvailableItems[random.Next(0, Restaurant.AvailableItems.Count)];
                        
                        var orderQuantity = 1;
                        var n = random.Next(10);
                        if (n < 4) orderQuantity++;
                        if (n < 2) orderQuantity++;

                        try
                        {
                            OrderFood(foodToOrder, orderQuantity);
                        } catch (Exception e)
                        {
                            Console.WriteLine($"{Name} couldn't buy what they wanted: {e.Message}");
                        }
                        
                        Thread.Sleep(100);
                        break;

                    case RestaurantActivities.Talk:
                        Console.WriteLine($"{Name} is talking with someone...");
                        Thread.Sleep(100);
                        break;

                    case RestaurantActivities.Leave:
                        lock (this.Restaurant)
                        {
                            if (this.Restaurant.IsOpen)
                            {
                                LeaveRestaurant();
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }

                Thread.Sleep(100);
            }

            WalkOut();
        }

        public void LeaveRestaurant()
        {
            if (!inRestaurant)
            {
                throw new InvalidOperationException($"{Name} can't leave a restaurant, they are not in one!");
            }

            Console.WriteLine($"{Name} is leaving...");
            
            Restaurant.Leave(this);
            this.inRestaurant = false;
        }

        public void DoSomething()
        {
            WalkOut();
            var stayingOut = true;
            while (stayingOut)
            {
                var nextActivity = GetRandomActivity();
                switch (nextActivity)
                {
                    case StudentActivities.Walk:
                        WalkOut();
                        break;
                    case StudentActivities.VisitRestaurant:
                        VisitRestaurant();
                        break;
                    case StudentActivities.GoHome:
                        stayingOut = false;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                Thread.Sleep(100);
            }

            Console.WriteLine($"{Name} is going home.");
        }

        public Student(string name, int age, decimal budget)
        {
            this.Name = name;
            this.Age = age;
            this.ballance = budget;
        }

        public Dictionary<Food, int> BoughtProducts { get; } = new();

        public void OrderFood(Food food, int quantity)
        {
            if (!inRestaurant)
            {
                throw new InvalidOperationException($"{Name} is not in a restaurant!");
            }

            var foodPrice = food.Price * quantity;

            if (foodPrice > this.Ballance)
            {                                
                throw new InvalidOperationException($"{Name} wanted to buy {quantity} of {food.Name} but had not enough money!");
            }

            //TODO: Change name
            var successfullyBoughtFood = Restaurant.ServeOrder(food, quantity);

            if (successfullyBoughtFood)
            {
                this.ballance -= food.Price * quantity;

                Console.WriteLine($"{Name} bought {food.Name} x {quantity}. They are eating now... (They have {Ballance:F2} lv. left)");
            }
            else
            {
                Console.WriteLine($"{Name} couldn't buy {food.Name} x {quantity}: Restaurant didn't have enough.");
            }
        }

        public void OrderFood(Food f) => OrderFood(f, 1);

        public override string ToString()
        {
            return Name;
        }
    }
}
