using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCart? GetByUserId(Guid userId);
        ShoppingCart? GetById(Guid id);
        ShoppingCart Insert(ShoppingCart shoppingCart);
        ShoppingCartDTO GetByUserIdWithIncludedPrducts(Guid userId);
        List<ShoppingCart> GetAll();
        void DeleteProductFromShoppingCart(Guid productInShoppingCartId);
        bool PrintReceipt(Guid userId);
        
    }
}
