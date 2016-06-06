using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class CartItem
    {
        public string ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
