using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO.Dashboard_DTOs
{
    public class CategoryChartDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public double Quantity { get; set; }
    }
}
