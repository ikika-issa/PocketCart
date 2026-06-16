using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class ProductInShoppingCart : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
        public double quantity {  get; set; }
        public string? unit {  get; set; } 
    }
}
