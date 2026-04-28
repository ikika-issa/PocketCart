using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class ProductInCategory : BaseEntity
    {
        public Guid ProductId {  get; set; }
        public Product? Product { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
