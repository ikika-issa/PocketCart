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
        [Display(Name = "Product Name")]
        public required string ProductName { get; set; }
        [Required]
        [Display(Name = "Price")]
        public double ProductPrice { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }
        [Required]
        [Display(Name = "Expires")]
        public DateOnly ExpirationDate { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public double quantity { get; set; }
        [Required]
        public Guid ManufacturerId { get; set; }
        [Display(Name = "Manufacturer")]
        public Manufacturer? Manufacturer { get; set; }
        public virtual ICollection<ShoppingCart>? AllShoppingCarts { get; set; }
        [Display(Name = "Barcode")]
        public string? Barcode { get; set; }
        [Display(Name = "Barcode Image")]
        public string? BarcodeImagePath { get; set; }
    }
}
