using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class Product : BaseEntity
    {
        [Required]
        public string? ProductName { get; set; }
        [Required]
        public double ProductPrice { get; set; }
        public Guid? CategoryId { get; set; }
        [Required]
        public string? CategoryName { get; set; }
        [Required]
        public DateOnly ExpirationDate { get; set; }
        public double? quantity { get; set; }
        public string? Manufacturer { get; set; }
        public virtual ICollection<ShoppingCart>? AllShoppingCarts { get; set; }
        public string? Barcode { get; set; }
        public string? BarcodeImagePath { get; set; }
    }
}
