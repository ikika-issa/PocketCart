using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class Deal : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        public DealType DealType { get; set; }

        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public double? DiscountPrice { get; set; }

        public Guid? BundleProductId { get; set; }
        public Product? BundleProduct { get; set; }

        public double? BundlePrice { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
