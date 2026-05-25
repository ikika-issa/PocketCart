using PocketCartApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.API.Interface
{
    public interface IOpenFoodFactsService
    {
        Task ImportSampleProductsAsync(string webRootPath);
    }
}
