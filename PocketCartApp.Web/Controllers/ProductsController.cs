using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository;
using PocketCartApp.Service.API.Interface;
using PocketCartApp.Service.Interface;

namespace PocketCartApp.Web.Controllers
{
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


        // GET: Products
        public IActionResult Index()
        {
            ViewBag.Categories = _categoryService.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToList();

            return View(_productService.GetAll());
        }

        // GET: Products/Details/5
        public IActionResult Details(Guid id)
        {

            var product = _productService.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
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

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("ProductName,ProductPrice,CategoryId, ExpirationDate, quantity, ManufacturerId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _productService.Update(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(Guid id)
        {
            var product = _productService.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var product = _productService.GetById(id);

            if (product != null)
            {
                _productService.DeleteById(id);
            }

            return RedirectToAction(nameof(Index));
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
                    product.ProductPrice = double.Parse(value);
                    break;

                case "quantity":
                    product.quantity = double.Parse(value);
                    break;

                case "CategoryId":
                    product.CategoryId = Guid.Parse(value);

                    var category = _categoryService.GetById(product.CategoryId.Value);
                    product.CategoryName = category?.CategoryName;
                    break;

                default:
                    return BadRequest("Invalid field");
            }

            _productService.Update(product);

            return Ok();
        }
    }
}
