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
    public class ManufacturersController : Controller
    {
        private IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        // GET: Manufacturers
        public IActionResult Index()
        {
            var manufacturers = _manufacturerService.GetAll();
            return View(manufacturers);
        }

        // GET: Manufacturers/Details/5
        public IActionResult Details(Guid id)
        {
            var manufacturer = _manufacturerService.GetById(id);    
            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // GET: Manufacturers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manufacturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ManufacturerName,Id")] Manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                _manufacturerService.Insert(manufacturer);

                return RedirectToAction(nameof(Index));
            }
            return View(manufacturer);
        }

        // GET: Manufacturers/Edit/5
        public IActionResult Edit(Guid id)
        {
            var manufacturer = _manufacturerService.GetById(id);

            if (manufacturer == null)
            {
                return NotFound();
            }
            return View(manufacturer);
        }

        // POST: Manufacturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("ManufacturerName,Id")] Manufacturer manufacturer)
        {
            if (id != manufacturer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _manufacturerService.Update(manufacturer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManufacturerExists(manufacturer.Id))
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
            return View(manufacturer);
        }

        // GET: Manufacturers/Delete/5
        public IActionResult Delete(Guid id)
        {
            var manufacturer = _manufacturerService.GetById(id);

            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // POST: Manufacturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var manufacturer = _manufacturerService.GetById(id);

            if (manufacturer != null)
            {
                _manufacturerService.DeleteById(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ManufacturerExists(Guid id)
        {
            return _manufacturerService.GetById(id) != null;
        }
    }
}
