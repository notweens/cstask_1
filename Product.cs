using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Product
    {
        private uint id;
        private string name;
        private uint price;

        public Product(uint id, string name, uint price)
        {
            this.Id = id;
            this.Name = name;
            this.Price = price;
        }

        public uint Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public uint Price { get => price; set => price = value; }

        public override bool Equals(object obj)
        {
            var product = obj as Product;
            return product != null &&
                   Id == product.Id &&
                   Name == product.Name &&
                   Price == product.Price;
        }

        public override int GetHashCode()
        {
            var hashCode = 712951391;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Price.GetHashCode();
            return hashCode;
        }
    }
}
