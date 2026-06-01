using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class ProductInReceipt : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid ReceiptId { get; set; }
        public int Quantity { get; set; }
        public virtual Product? OrderedProduct { get; set; }
        public virtual Receipt? Receipt { get; set; }
    }
}
