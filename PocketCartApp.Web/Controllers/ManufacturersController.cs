using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Repository;
using PocketCartApp.Service.Implementation;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketCartApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
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

        [HttpPost]
        public IActionResult UpdateInline(Guid id, string field, string value)
        {
            var manufacturer = _manufacturerService.GetById(id);

            if (manufacturer == null)
                return NotFound();

            switch (field)
            {
                case "ManufacturerName":
                    manufacturer.ManufacturerName = value;
                    break;

                default:
                    return BadRequest("Invalid field");
            }

            _manufacturerService.Update(manufacturer);

            return Ok();
        }
    }
}
