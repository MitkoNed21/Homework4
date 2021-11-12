using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework4
{
    class Supplier
    {
        public string Name { get; set; }

        public void SupplyRestaurant(Food food, int quantity, Restaurant restaurant)
        {
            try
            {
                restaurant.AddToStock(food, quantity);
                Console.WriteLine($"{Name} supplied restaurant with {quantity} of {food.Name}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Name} couldn't supply restaurant with {food.Name}: {e.Message}");
            }
        }

        public void SupplyRestaurant(Food f, Restaurant shop)
        {
            shop.AddToStock(f);
        }

        public Supplier(string name) => this.Name = name;

        public override string ToString()
        {
            return Name;
        }
    }
}
