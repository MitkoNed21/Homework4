using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework4
{
    class Food
    {
        private decimal price = 0;
        private decimal discount = 0;

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price
        {
            get
            {
                if (IsDiscounted)
                {
                    return price - discount;
                }

                return price;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Price), "Price can't be negative!");
                }

                this.price = value;
            }
        }
        public bool IsDiscounted { get; set; } = false;
        public decimal Discount
        {
            get => discount;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Discount), "Discount can't be negative!");
                }

                if (value > price)
                {
                    throw new ArgumentOutOfRangeException(nameof(Discount), "Discount can't be greater than actual price!");
                }

                this.discount = value;
            }
        }

        public Food(string name, string description, decimal price)
        {
            this.Name = name;
            this.Description = description;
            this.Price = price;
        }

        public Food(string name, string description, decimal price, decimal discount)
            : this(name, description, price)
        {
            this.IsDiscounted = true;
            this.Discount = discount;
        }

        public Food(string name, decimal price)
            : this(name, "", price) { }

        public Food(string name, decimal price, decimal discount)
            : this(name, "", price, discount) { }

        public override string ToString()
        {
            var result = $"{Name} ({Price:F2} lv.)";
            if (this.IsDiscounted)
            {
                result += $"; Regular price: {price:F2} lv.";
            }
            return result;
        }
    }
}
