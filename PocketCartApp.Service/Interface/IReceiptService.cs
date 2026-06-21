using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IReceiptService
    {
        List<Receipt> GetAll();
        List<Receipt> GetAllByUserId(string userId);
        Receipt? GetById(Guid id);
        Receipt Insert(Receipt receipt);
        Receipt Update(Receipt receipt);
    }
}
