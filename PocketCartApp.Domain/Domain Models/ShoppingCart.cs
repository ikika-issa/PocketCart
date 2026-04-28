using PocketCartApp.Domain.Identity_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class ShoppingCart : BaseEntity
    {
        public string? CashierOnShift { get; set; } //LOGGED IN USER ID
        public PocketCartApplicationUser? Cashier { get; set; }
        public virtual ICollection<ProductInShoppingCart>? ProductsInCart { get; set; }
    }
}
