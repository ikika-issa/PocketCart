using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product? GetById(Guid id);
        Product Insert(Product product);
        Product Update(Product product);
        Product DeleteById(Guid id);
        AddToCartDTO GetSelectedShoppingCartProduct(Guid id);
        void AddProductToShoppingCart(Guid id, Guid cashierId, int quantity);
    }
}
