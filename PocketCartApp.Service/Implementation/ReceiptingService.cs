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
    public class ReceiptingService : IReceiptService
    {
        private readonly IRepository<Receipt> _receiptRepository;

        public ReceiptingService(IRepository<Receipt> receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }

        public List<Receipt> GetAll()
        {
            return _receiptRepository.GetAll(selector: x => x, 
                include: x => x.Include(z => z.ShoppingCart).ThenInclude(sp => sp.Cashier)).ToList();
        }

        public List<Receipt> GetAllByUserId(string userId)
        {
            return _receiptRepository.GetAll(selector: x => x, predicate: x => x.userId == userId, 
                include: x => x.Include(z => z.ShoppingCart).ThenInclude(sp => sp.Cashier)).ToList();
        }

        public Receipt? GetById(Guid id)
        {
            return _receiptRepository.Get(selector: x => x, predicate: x => x.Id.Equals(id));
        }

        public Receipt Insert(Receipt receipt)
        {
            receipt.Id = Guid.NewGuid();
            return _receiptRepository.Insert(receipt);
        }

        public Receipt Update(Receipt receipt)
        {
            return _receiptRepository.Update(receipt);
        }
    }
}
