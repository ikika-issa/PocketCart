using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class Manufacturer : BaseEntity
    {
        [Required]
        public required string ManufacturerName { get; set; }
        public virtual ICollection<Product>? Products { get; set; } //this is added for charts to be able to be created later on
    }
}
