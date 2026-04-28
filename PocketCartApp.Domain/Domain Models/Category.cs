using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class Category : BaseEntity
    {
        public string? CategoryName { get; set; }
        public virtual ICollection<Product>? ProductsInCategory { get; set; }
    }
}
