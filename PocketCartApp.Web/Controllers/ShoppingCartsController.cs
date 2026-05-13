using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Repository;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PocketCartApp.Web.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartsController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }


        // GET: ShoppingCarts
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



    }
}
