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
        Product? GetByBarcode(string barcode);
        Product Insert(Product product, string webRootPath);
        Product Update(Product product);
        Product DeleteById(Guid id);
        byte[] ExportProducts();
        AddToCartDTO GetSelectedShoppingCartProduct(Guid id);
        void AddProductToShoppingCartByBarcode(string barcode, string cashierId);
        void AddProductToShoppingCart(Guid id, string cashierId, int quantity);
    }
}
