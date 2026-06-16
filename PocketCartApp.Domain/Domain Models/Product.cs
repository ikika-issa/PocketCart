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
        public required string ProductName { get; set; }
        [Required]
        public double ProductPrice { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        [Required]
        public DateOnly ExpirationDate { get; set; }
        [Required]
        public double quantity { get; set; }
        [Required]
        public Guid ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public virtual ICollection<ShoppingCart>? AllShoppingCarts { get; set; }
        public required string Barcode { get; set; }
        public required string BarcodeImagePath { get; set; }
    }
}
