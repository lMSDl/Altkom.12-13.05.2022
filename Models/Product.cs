using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product : Entity
    {
        //private byte[] timestamp;
        //private byte[] _timestamp;
        private byte[] m_timestamp;

        //Konfigurujemy token współbieżności
        //[ConcurrencyCheck]
        public string Name { get; set; }
        public float Price { get; set; }
        //public virtual IEnumerable<Order> Orders { get; set; } // virtual wymagany przez ProxyLazyLoading
        public IEnumerable<Order> Orders { get; set; }

        private string _description;

        //Konfigurujemy token współbieżności za pomocą sygnatury czasowej
        //[Timestamp]
        public byte[] Timestamp { get => m_timestamp; }

        public override string ToString()
        {
            return _description;
        }
    }
}
