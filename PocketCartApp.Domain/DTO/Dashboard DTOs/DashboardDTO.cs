using PocketCartApp.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO.Dashboard_DTOs
{
    public class DashboardDTO
    {
        public int TotalProducts { get; set; }
        public int NearExpiryCount { get; set; }
        public int ExpiredCount { get; set; }
        public int LowStockCount { get; set; }
        public double TotalInventoryValue { get; set; }

        public List<ProductExpiryDTO> NearExpiryProducts { get; set; } = new();
        public List<CategoryChartDTO> ProductsByCategory { get; set; } = new();
        public List<ManufacturerChartDTO> ProductsByManufacturer { get; set; } = new();
        public List<Deal> ActiveDeals { get; set; } = new();
    }
}
