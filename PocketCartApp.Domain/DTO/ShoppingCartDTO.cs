using PocketCartApp.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public List<ProductInShoppingCart>? Products { get; set; }
        public double TotalPrice { get; set; }
    }
}
