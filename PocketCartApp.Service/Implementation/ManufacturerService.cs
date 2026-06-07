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
    public class ManufacturerService : IManufacturerService
    {
        private readonly IRepository<Manufacturer> _manufacturerRepository;

        public ManufacturerService(IRepository<Manufacturer> manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public Manufacturer DeleteById(Guid id)
        {
            var manufacturer = _manufacturerRepository.Get(x => x, predicate: x => x.Id == id);

            if (manufacturer != null)
            {
                _manufacturerRepository.Delete(manufacturer);
            }

            return manufacturer!;
        }

        public List<Manufacturer> GetAll()
        {
            return _manufacturerRepository.GetAll(selector: x => x).ToList();
        }

        public Manufacturer GetById(Guid id)
        {
            return _manufacturerRepository.Get(x => x, predicate: x => x.Id == id)!;
        }

        public Manufacturer Insert(Manufacturer manufacturer)
        {
            manufacturer.Id = Guid.NewGuid();
            _manufacturerRepository.Insert(manufacturer);
            return manufacturer;
        }

        public Manufacturer Update(Manufacturer manufacturer)
        {
            return _manufacturerRepository.Update(manufacturer);
        }
    }
}
