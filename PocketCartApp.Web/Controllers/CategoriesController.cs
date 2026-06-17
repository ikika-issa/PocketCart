using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Repository;
using PocketCartApp.Service.API.Interface;
using PocketCartApp.Service.Implementation;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketCartApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly ICategoryAPIService categoryAPIService;

        public CategoriesController(ICategoryService categoryService, ICategoryAPIService categoryAPIService)
        {
            this.categoryService = categoryService;
            this.categoryAPIService = categoryAPIService;
        }

        // GET: Categories
        public IActionResult Index()
        {
            //var categories = await categoryAPIService.FetchAllCategories();

            return View(categoryService.GetAll());
        }

        // GET: Categories/Details/5
        public IActionResult Details(Guid id)
        { 
            var category = categoryService.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CategoryName,Id")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                categoryService.Insert(category);
                
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(Guid id)
        {
            var category = categoryService.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var category = categoryService.GetById(id);

            if (category != null)
            {
                categoryService.DeleteById(id);
            }

            return RedirectToAction(nameof(Index));
        }

        //API
        public async Task<IActionResult> FetchCategories()
        {
            await categoryAPIService.FetchAllCategories();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateInline(Guid id, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return BadRequest();

            var category = categoryService.GetById(id);

            if (category == null)
                return NotFound();

            category.CategoryName = value;

            categoryService.Update(category);

            return Ok();
        }
    }
}
