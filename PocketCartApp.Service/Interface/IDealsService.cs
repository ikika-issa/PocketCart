using PocketCartApp.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IDealsService
    {
        List<Deal> GetAll();
        List<Deal> GetActiveDeals();
        Deal? GetById(Guid id);
        Deal Insert(Deal deal);
        Deal Update(Deal deal);
        void DeleteById(Guid id);
        Deal? GetActiveDealsForProduct(Guid productId);
    }
}
