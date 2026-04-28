using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Implementation
{
    public class CategoryService : ICategoryService
    {

        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Category DeleteById(Guid id)
        {
            var category = GetById(id);
            if (category != null)
            {
                throw new Exception("Category not found");
            }

            _categoryRepository.Delete(category!);
            return category!;
        }

        public List<Category> GetAll()
        {
            return _categoryRepository.GetAll(selector: x => x).ToList();
        }

        public Category? GetById(Guid id)
        {
            return _categoryRepository.Get(selector: x => x, predicate: x => x.Id == id);
        }

        public Category Insert(Category category)
        {
            category.Id = Guid.NewGuid();
            return _categoryRepository.Insert(category);
        }

        public Category Update(Category category)
        {
            return _categoryRepository.Update(category);
        }
    }
}
