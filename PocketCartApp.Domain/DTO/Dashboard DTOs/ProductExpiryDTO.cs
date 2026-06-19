using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO.Dashboard_DTOs
{
    public class ProductExpiryDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public DateOnly ExpirationDate { get; set; }
        public int DaysLeft { get; set; }
        public double Quantity { get; set; }
    }
}
