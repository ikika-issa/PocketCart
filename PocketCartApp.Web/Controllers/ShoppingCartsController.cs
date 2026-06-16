using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
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


        // GET: ShoppingCarts
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_shoppingCartService.GetAll());
        }

        // GET: ShoppingCarts/Details/5
        public IActionResult Details(Guid id)
        {
            var shoppingCart = _shoppingCartService.GetById(id);

            if (shoppingCart == null)
            {
                return NotFound();
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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


        // POST: ShoppingCarts/Delete/5
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
            return View(shoppingCart);
        }

        public async Task<IActionResult> ClearCart(string cashierCode)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return Unauthorized();

            if (user.cashierId != cashierCode)
            {
                TempData["Error"] = "Invalid Cashier ID.";
                return RedirectToAction(nameof(CartIndex));
            }

            _shoppingCartService.ClearCart(userId!);

            TempData["Success"] = "Cart cleared successfully.";

            return RedirectToAction(nameof(CartIndex));
        }

        public IActionResult Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _shoppingCartService.PrintReceipt(userId);
            _shoppingCartService.ClearCart(userId);

            return RedirectToAction(nameof(CartIndex));
        }
    }
}