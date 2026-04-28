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
        ShoppingCartDTO GetByUserIdWithIncludedPrducts(Guid userId);
        void DeleteProductFromShoppingCart(Guid productInShoppingCartId);
    }
}
