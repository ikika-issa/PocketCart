using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO
{
    public class OpenFoodFactsProductDTO
    {
        public string? Product_Name { get; set; }
        public string? Brands { get; set; }
        public List<string>? Category_Tags { get; set; }
    }
}
