using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface ICategoryService
    {
        List<Category> GetAll();
        Category? GetById(Guid id);
        Category Insert(Category category);
        Category Update(Category category);
        Category DeleteById(Guid id);
    }
}
