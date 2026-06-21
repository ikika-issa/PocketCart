using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.API.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PocketCartApp.Service.API.Implementation
{
    public class CategoryAPIService : ICategoryAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ICategoryService _categoryService;

        public CategoryAPIService(HttpClient httpClient, ICategoryService categoryService)
        {
            _httpClient = httpClient;
            _categoryService = categoryService;
        }

        public async Task<List<Category>> FetchAllCategories()
        {
            var categories = new List<Category>();

            // cache existing data -> OGROMNO zabrzuvanje
            var existingCategories = _categoryService.GetAll().ToList();

            for (char c = 'a'; c <= 'z'; c++)
            {
                var response = await _httpClient.GetFromJsonAsync<MealDetailResponseWrapperDTO>(
                    $"https://www.themealdb.com/api/json/v1/1/search.php?f={c}");

                if (response?.Meals == null)
                    continue;

                foreach (var meal in response.Meals)
                {
                    /// ✅ CATEGORY
                    var category = existingCategories
                        .FirstOrDefault(x => x.CategoryName == meal.StrCategory);

                    if (category == null)
                    {
                        category = new Category
                        {
                            Id = Guid.NewGuid(),
                            CategoryName = meal.StrCategory
                        };

                        _categoryService.Insert(category);
                        existingCategories.Add(category);
                    }
                }
            }

            return existingCategories;
        }
    }
}
