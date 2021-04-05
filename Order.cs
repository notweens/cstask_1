using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Order
    {
        private uint id;
        private uint buyer;
        private DateTime date;
        private Dictionary<Product, uint> items;

        public Order(uint id, uint buyer, DateTime date, Dictionary<Product, uint> items)
        {
            this.Id = id;
            this.Buyer = buyer;
            this.Date = date;
            this.Items = items;
        }

        public uint Id { get => id; set => id = value; }
        public uint Buyer { get => buyer; set => buyer = value; }
        public DateTime Date { get => date; set => date = value; }
        public Dictionary<Product, uint> Items { get => items; set => items = value; }

        public override bool Equals(object obj)
        {
            var orders = obj as Order;
            return orders != null &&
                   Id == orders.Id &&
                   Buyer == orders.Buyer &&
                   Date == orders.Date &&
                   Items == orders.Items;
        }

        public override int GetHashCode()
        {
            var hashCode = 932362998;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(Buyer);
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Product, uint>>.Default.GetHashCode(Items);
            return hashCode;
        }
    }
}
