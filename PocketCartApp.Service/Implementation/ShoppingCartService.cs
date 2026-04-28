using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
        }

        public void DeleteProductFromShoppingCart(Guid productInShoppingCartId)
        {

            var prodictInShoppingCart = _productInShoppingCartRepository.Get(selector: x => x,
                                                                             predicate: x => x.Id.Equals(productInShoppingCartId));

            if (prodictInShoppingCart == null)
            {
                throw new Exception("Product in shopping cart not found");
            }

            _productInShoppingCartRepository.Delete(prodictInShoppingCart);
        }

        public ShoppingCart? GetByUserId(Guid userId)
        {
            return _shoppingCartRepository.Get(selector: x => x,
                                                       predicate: x => x.CashierOnShift!.Equals(userId.ToString()));
        }

        public ShoppingCartDTO GetByUserIdWithIncludedPrducts(Guid userId)
        {
            var userCart = _shoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.CashierOnShift!.Equals(userId.ToString()),
                include: x => x
                    .Include(z => z.ProductsInCart!)
                    .ThenInclude(p => p.Product!.ProductName!)
            );

            var allProducts = userCart!.ProductsInCart!;

            double totalPrice = 0.0;

            foreach (var item in allProducts)
            {
                totalPrice += item.quantity * item.Product!.ProductPrice;
            }

            ShoppingCartDTO model = new ShoppingCartDTO
            {
                Products = allProducts.ToList(),
                TotalPrice = totalPrice
            };

            return model;
        }

    }
}
