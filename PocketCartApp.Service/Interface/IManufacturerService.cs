using PocketCartApp.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IManufacturerService
    {
        List<Manufacturer> GetAll();
        Manufacturer GetById(Guid id);
        Manufacturer Insert(Manufacturer manufacturer);
        Manufacturer Update(Manufacturer manufacturer);
        Manufacturer DeleteById(Guid id);
    }
}
