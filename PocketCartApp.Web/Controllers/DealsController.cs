using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Service.Implementation;
using PocketCartApp.Service.Interface;

[Authorize(Roles = "Admin,Manager")]
public class DealsController : Controller
{
    private readonly IDealsService _dealService;
    private readonly IProductService _productService;

    public DealsController(IDealsService dealService, IProductService productService)
    {
        _dealService = dealService;
        _productService = productService;
    }

    public IActionResult Index()
    {
        return View(_dealService.GetAll());
    }

    public IActionResult Create()
    {
        LoadProducts();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Deal deal)
    {
        if (!ModelState.IsValid)
        {
            LoadProducts();
            return View(deal);
        }

        _dealService.Insert(deal);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(Guid id)
    {
        _dealService.DeleteById(id);
        return RedirectToAction(nameof(Index));
    }

    private void LoadProducts()
    {
        ViewBag.Products = _productService.GetAll()
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.ProductName} - {p.Barcode}"
            })
            .ToList();
    }
}