using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order : Entity
    {
        private ILazyLoader _lazyLoader;
        private ICollection<Product> products;

        public Order()
        {
        }

        private Order(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public OrderType OrderType { get; set; }

        public DateTime DateTime { get; set; }
        //public virtual ICollection<Product> Products { get; set; }  // virtual wymagany przez ProxyLazyLoading
        public ICollection<Product> Products { get => _lazyLoader.Load(this, ref products); set => products = value; }
    }
}
