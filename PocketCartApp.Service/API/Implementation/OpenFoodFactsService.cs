using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.API.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.API.Implementation
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IBarcodeService _barcodeService;

        public OpenFoodFactsService(
            HttpClient httpClient,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IBarcodeService barcodeService)
        {
            _httpClient = httpClient;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _barcodeService = barcodeService;
        }

        public async Task ImportSampleProductsAsync(string webRootPath)
        {
            var categoriesToImport = new[]
            {
            "en:beverages",
            "en:snacks",
            "en:dairy",
            "en:breakfast-cereals",
            "en:chocolates"
        };

            var random = new Random();

            foreach (var categoryTag in categoriesToImport)
            {
                var url = $"https://world.openfoodfacts.org/api/v2/search?categories_tags={categoryTag}" +
                    $"&page_size=10&fields=product_name,brands,categories_tags";

                try
                {
                    using var httpResponse = await _httpClient.GetAsync(url);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"OpenFoodFacts failed: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                        await Task.Delay(3000);
                        continue;
                    }

                    var response = await httpResponse.Content
                        .ReadFromJsonAsync<OpenFoodFactsResponseDTO>();

                    if (response?.Products == null || !response.Products.Any())
                        continue;

                    foreach (var apiProduct in response.Products)
                    {
                        if (string.IsNullOrWhiteSpace(apiProduct.Product_Name))
                            continue;

                        var manufacturerName = apiProduct.Brands?
                            .Split(',')
                            .FirstOrDefault()
                            ?.Trim();

                        if (string.IsNullOrWhiteSpace(manufacturerName))
                            manufacturerName = "Unknown";

                        var categoryName = categoryTag
                            .Replace("en:", "")
                            .Replace("-", " ");

                        var category = _categoryRepository.Get(
                            selector: x => x,
                            predicate: x => x.CategoryName == categoryName);

                        if (category == null)
                        {
                            category = new Category
                            {
                                Id = Guid.NewGuid(),
                                CategoryName = categoryName
                            };

                            _categoryRepository.Insert(category);
                        }

                        var manufacturer = _manufacturerRepository.Get(
                            selector: x => x,
                            predicate: x => x.ManufacturerName == manufacturerName);

                        if (manufacturer == null)
                        {
                            manufacturer = new Manufacturer
                            {
                                Id = Guid.NewGuid(),
                                ManufacturerName = manufacturerName
                            };

                            _manufacturerRepository.Insert(manufacturer);
                        }

                        var barcode = _barcodeService.GenerateEAN13();

                        var product = new Product
                        {
                            Id = Guid.NewGuid(),
                            ProductName = apiProduct.Product_Name,
                            ProductPrice = random.Next(30, 350),
                            CategoryId = category.Id,
                            CategoryName = category.CategoryName,
                            ManufacturerId = manufacturer.Id,
                            Manufacturer = manufacturer,
                            ExpirationDate = DateOnly.FromDateTime(
                                DateTime.Now.AddMonths(random.Next(3, 24))
                            ),
                            quantity = random.Next(10, 100),
                            Barcode = barcode,
                            BarcodeImagePath = _barcodeService.GenerateBarcodeImage(
                                barcode,
                                webRootPath
                            )
                        };

                        _productRepository.Insert(product);
                    }

                    await Task.Delay(1500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OpenFoodFacts exception: {ex.Message}");
                    continue;
                }
            }
        }
    }
}
