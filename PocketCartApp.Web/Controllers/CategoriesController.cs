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
    [Authorize(Roles = "Admin, Manager")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly ICategoryAPIService categoryAPIService;

        public CategoriesController(ICategoryService categoryService, ICategoryAPIService categoryAPIService)
        {
            this.categoryService = categoryService;
            this.categoryAPIService = categoryAPIService;
        }

       
        public IActionResult Index()
        {
            //var categories = await categoryAPIService.FetchAllCategories();

            return View(categoryService.GetAll());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid id)
        {
            var category = categoryService.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
