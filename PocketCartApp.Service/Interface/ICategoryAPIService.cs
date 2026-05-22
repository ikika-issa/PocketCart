using PocketCartApp.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface ICategoryAPIService
    {
        public Task<List<Category>> FetchAllCategories();
    }
}
