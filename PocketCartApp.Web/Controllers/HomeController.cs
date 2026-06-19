using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PocketCartApp.Domain;
using PocketCartApp.Domain.DTO.Dashboard_DTOs;
using PocketCartApp.Service.Interface;
using System.Diagnostics;

namespace PocketCartApp.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IDealsService _dealService;

        public HomeController(ILogger<HomeController> logger, IProductService productService,
            IDealsService dealsService)
        {
            _logger = logger;
            _productService = productService;
            _dealService = dealsService;
        }

        public IActionResult Index()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var nearLimit = today.AddDays(30);

            var products = _productService.GetAll();

            var model = new DashboardDTO
            {
                TotalProducts = products.Count,
                NearExpiryCount = products.Count(p =>
                    p.ExpirationDate >= today &&
                    p.ExpirationDate <= nearLimit),

                ExpiredCount = products.Count(p =>
                    p.ExpirationDate < today),

                LowStockCount = products.Count(p =>
                    p.quantity <= 5),

                TotalInventoryValue = products.Sum(p =>
                    p.ProductPrice * (p.quantity)),

                NearExpiryProducts = products
                    .Where(p => p.ExpirationDate >= today &&
                                p.ExpirationDate <= nearLimit)
                    .OrderBy(p => p.ExpirationDate)
                    .Select(p => new ProductExpiryDTO
                    {
                        ProductName = p.ProductName,
                        ExpirationDate = p.ExpirationDate,
                        DaysLeft = p.ExpirationDate.DayNumber - today.DayNumber,
                        Quantity = p.quantity
                    })
                    .ToList(),

                ProductsByCategory = products
                    .GroupBy(p => p.CategoryName ?? "Uncategorized")
                    .Select(g => new CategoryChartDTO
                    {
                        CategoryName = g.Key,
                        Quantity = g.Sum(p => p.quantity)
                    })
                    .ToList(),

                ActiveDeals = _dealService.GetAll()
                    .Where(d => d.IsActive &&
                                d.StartDate <= today &&
                                d.EndDate >= today)
                    .ToList()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
