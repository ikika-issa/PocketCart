using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository;
using PocketCartApp.Service.API.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PocketCartApp.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly IOpenFoodFactsService _productImportService;
        private readonly IManufacturerService _manufacturerService;

        public ProductsController(IProductService productService, ICategoryService categoryService, 
            IWebHostEnvironment environment, IOpenFoodFactsService productImportService, 
            IManufacturerService manufacturerService   )
        {
            _productService = productService;
            _categoryService = categoryService;
            _environment = environment;
            _productImportService = productImportService;
            _manufacturerService = manufacturerService;
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewBag.Categories = _categoryService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToList();

            ViewBag.Manufacturers = _manufacturerService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ManufacturerName
                })
                .ToList();

            return View(_productService.GetAll());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToList();

            ViewBag.Manufacturers = _manufacturerService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ManufacturerName
                })
                .ToList();

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ProductName,ProductPrice,CategoryId,ExpirationDate, quantity, ManufacturerId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productService.Insert(product, _environment.WebRootPath);
             
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _categoryService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToList();

            ViewBag.Manufacturers = _manufacturerService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.ManufacturerName
                })
                .ToList();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _productService.DeleteById(id);
            return Ok();
        }

        private bool ProductExists(Guid id)
        {
            return _productService.GetById(id) != null;
        }

        public IActionResult AddProductToCart(AddToCartDTO model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _productService.AddProductToShoppingCart(model.SelectedProductId, userId!, model.Quantity);

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public IActionResult AddByBarcode(string barcode)
        {
            var cashierId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(cashierId))
                return Unauthorized();

            try
            {
                _productService.AddProductToShoppingCartByBarcode(barcode, cashierId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("CartIndex", "ShoppingCarts");
        }

        //API IMPLEMENTATION
        public async Task<IActionResult> ImportSampleProducts()
        {
            await _productImportService.ImportSampleProductsAsync(_environment.WebRootPath);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateInline(Guid id, string field, string value)
        {
            var product = _productService.GetById(id);

            if (product == null)
                return NotFound();

            switch (field)
            {
                case "ProductName":
                    product.ProductName = value;
                    break;

                case "ProductPrice":
                    if (string.IsNullOrWhiteSpace(value))
                        return BadRequest("Price is empty");

                    if (!double.TryParse(value, out double price))
                        return BadRequest("Invalid price");

                    product.ProductPrice = price;
                    break;

                case "quantity":
                    product.quantity = double.Parse(value);
                    break;

                case "CategoryId":
                    product.CategoryId = Guid.Parse(value);

                    var category = _categoryService.GetById(product.CategoryId);
                    product.CategoryName = category?.CategoryName;
                    break;

                case "ManufacturerId":
                    product.ManufacturerId = Guid.Parse(value);

                    var manufacturer = _manufacturerService.GetById(product.ManufacturerId);

                    if (manufacturer == null)
                        return BadRequest();

                    product.Manufacturer = manufacturer;
                    break;

                case "ExpirationDate":
                    if (!DateOnly.TryParse(value, out DateOnly expirationDate))
                        return BadRequest();

                    product.ExpirationDate = expirationDate;
                    break;

                default:
                    return BadRequest("Invalid field");
            }

            _productService.Update(product);

            return Ok();
        }

        [HttpPost]
        public IActionResult AddByBarcodeAjax([FromBody] BarcodeRequest model)
        {
            var cashierId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(cashierId))
                return Unauthorized(new { success = false, message = "Unauthorized" });

            if (model == null || string.IsNullOrWhiteSpace(model.Barcode))
                return BadRequest(new { success = false, message = "Barcode is empty" });

            try
            {
                _productService.AddProductToShoppingCartByBarcode(model.Barcode, cashierId);
                var product = _productService.GetByBarcode(model.Barcode);
                return Ok(new { success = true, productName = product?.ProductName, price = product?.ProductPrice });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        public class BarcodeRequest
        {
            public string Barcode { get; set; } = string.Empty;
        }
    }
}
