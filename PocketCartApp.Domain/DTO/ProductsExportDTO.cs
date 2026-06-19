using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO
{
    public class ProductsExportDTO
    {
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public double quantity { get; set; }
    }
}
