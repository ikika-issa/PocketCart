using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Repository;
using PocketCartApp.Service.Interface;

namespace PocketCartApp.Web.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        // GET: Receipts
        public IActionResult Index()
        {
            return View(_receiptService.GetAll());
        }

        // GET: Receipts/Details/5
        public IActionResult Details(Guid id)
        {
            var receipt = _receiptService.GetById(id);

            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // GET: Receipts/Create
        public IActionResult Create()
        {
            //ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id");
            return View(_receiptService.GetAll());
        }

        // POST: Receipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ShoppingCartId,total,currency,Id")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                _receiptService.Insert(receipt);
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", receipt.ShoppingCartId);
            return View(receipt);
        }

        // GET: Receipts/Edit/5
        public IActionResult Edit(Guid id)
        {
            var receipt = _receiptService.GetById(id);
            if (receipt == null)
            {
                return NotFound();
            }
            //ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", receipt.ShoppingCartId);
            return View(receipt);
        }

        // POST: Receipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("ShoppingCartId,total,currency,Id")] Receipt receipt)
        {
            if (id != receipt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _receiptService.Update(receipt);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceiptExists(receipt.Id))
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
            //ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", receipt.ShoppingCartId);
            return View(receipt);
        }

        // GET: Receipts/Delete/5
        public IActionResult Delete(Guid id)
        {
            var receipt = _receiptService.GetById(id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var receipt = _receiptService.GetById(id);

            if (receipt != null)
            {
                _receiptService.DeleteById(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptExists(Guid id)
        {
            return _receiptService.GetById(id) != null; 
        }
    }
}
