using Microsoft.EntityFrameworkCore;
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
    public class DealService : IDealsService
    {
        private readonly IRepository<Deal> _dealRepository;

        public DealService(IRepository<Deal> dealRepository)
        {
            _dealRepository = dealRepository;
        }

        public List<Deal> GetAll()
        {
            return _dealRepository.GetAll(
                selector: x => x,
                include: x => x
                    .Include(d => d.Product!)
                    .Include(d => d.BundleProduct!)
            ).ToList();
        }

        public Deal? GetActiveDealsForProduct(Guid productId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return _dealRepository.Get(
                selector: x => x,
                predicate: x =>
                    x.IsActive &&
                    x.StartDate <= today &&
                    x.EndDate >= today &&
                    (x.ProductId == productId || x.BundleProductId == productId),
                include: x => x
                    .Include(d => d.Product!)
                    .Include(d => d.BundleProduct!)
            );
        }

        public List<Deal> GetActiveDeals()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return _dealRepository.GetAll(
                    selector: x => x,
                    predicate: x => x.IsActive &&
                                    x.StartDate <= today &&
                                    x.EndDate >= today,
                    include: x => x
                        .Include(d => d.Product!)
                        .Include(d => d.BundleProduct!)
                )
                .ToList();
        }

        public Deal? GetById(Guid id)
        {
            return _dealRepository.Get(
                selector: x => x,
                predicate: x => x.Id == id,
                include: x => x
                    .Include(d => d.Product!)
                    .Include(d => d.BundleProduct!)
            );
        }

        public Deal Insert(Deal deal)
        {
            ValidateDeal(deal);

            deal.Id = Guid.NewGuid();

            return _dealRepository.Insert(deal);
        }

        public Deal Update(Deal deal)
        {
            ValidateDeal(deal);

            return _dealRepository.Update(deal);
        }

        public void DeleteById(Guid id)
        {
            var deal = GetById(id);

            if (deal == null)
                throw new Exception("Deal not found.");

            _dealRepository.Delete(deal);
        }

        private void ValidateDeal(Deal deal)
        {
            if (string.IsNullOrWhiteSpace(deal.Title))
                throw new Exception("Deal title is required.");

            if (deal.StartDate > deal.EndDate)
                throw new Exception("Start date cannot be after end date.");

            if (deal.DealType == DealType.Priceoff)
            {
                if (deal.ProductId == null)
                    throw new Exception("Product is required for price-off deals.");

                if (deal.DiscountPrice == null || deal.DiscountPrice <= 0)
                    throw new Exception("Discount price must be greater than 0.");
            }

            if (deal.DealType == DealType.Bundle)
            {
                if (deal.ProductId == null || deal.BundleProductId == null)
                    throw new Exception("Both products are required for bundle deals.");

                if (deal.ProductId == deal.BundleProductId)
                    throw new Exception("Bundle products must be different.");

                if (deal.BundlePrice == null || deal.BundlePrice <= 0)
                    throw new Exception("Bundle price must be greater than 0.");
            }
        }
    }
}
