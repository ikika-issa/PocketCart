using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Domain.Identity_Models;
using PocketCartApp.Repository;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PocketCartApp.Web.Controllers
{
    [Authorize]
    public class ShoppingCartsController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly UserManager<PocketCartApplicationUser> _userManager;

        public ShoppingCartsController(IShoppingCartService shoppingCartService, UserManager<PocketCartApplicationUser> userManager)
        {
            _shoppingCartService = shoppingCartService;
            _userManager = userManager;
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_shoppingCartService.GetAll());
        }

        public IActionResult Details(Guid id)
        {
            var shoppingCart = _shoppingCartService.GetById(id);

            if (shoppingCart == null)
            {
                return NotFound();
            }
            return View(shoppingCart);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CashierOnShift,Id")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _shoppingCartService.Insert(shoppingCart);

                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _shoppingCartService.DeleteProductFromShoppingCart(id);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CartIndex()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var shoppingCart = _shoppingCartService.GetByUserIdWithIncludedPrducts(userId!);

            var dto = new ShoppingCartDTO
            {
                ProductsInCart = shoppingCart?.ProductsInCart?.ToList() ?? new List<ProductInShoppingCart>()
            };

            return View(dto);
        }

        [HttpPost]
        public IActionResult ClearCart(string cashierCode)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = _shoppingCartService.GetByUserIdWithIncludedProducts(userId);

            if (cart == null ||
                cart.ProductsInCart == null ||
                !cart.ProductsInCart.Any())
            {
                TempData["Error"] = "Cart is already empty.";
                return RedirectToAction("CartIndex");
            }

            _shoppingCartService.ClearCart(userId);

            TempData["Success"] = "Cart cleared successfully.";

            return RedirectToAction("CartIndex");
        }

        [HttpPost]
        public IActionResult UpdateCartQuantity(Guid productId, double quantity)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            _shoppingCartService.UpdateQuantity(userId, productId, quantity);

            return Ok();
        }

        public IActionResult Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var receiptId = _shoppingCartService.PrintReceipt(userId);

            if (receiptId == Guid.Empty)
            {
                TempData["Error"] = "Cart is empty. Cannot print receipt.";
                return RedirectToAction("CartIndex");
            }

            return RedirectToAction(nameof(CartIndex), new {receiptId});
        }

        public IActionResult Scanner()
        {
            return View();
        }
    }
}