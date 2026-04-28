using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IShoppingCartService _shoppingCartService;

        public ProductService(IRepository<Product> productRepository, 
            IRepository<ProductInShoppingCart> productInShoppingCartRepository, IShoppingCartService shoppingCartService)
        {
            _productRepository = productRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _shoppingCartService = shoppingCartService;
        }

        public void AddProductToShoppingCart(Guid id, Guid cashierId, int quantity)
        {
            var shoppingCart = _shoppingCartService.GetByUserId(cashierId);

            if (shoppingCart == null)
            {
                throw new Exception("Shopping cart not found");
            }

            var product = GetById(id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            UpdateCartItem(product, shoppingCart, quantity);

        }

        private void UpdateCartItem(Product product, ShoppingCart shoppingCart, int quantity)
        {
            var existingProduct = GetProductInShoppingCart(product.Id, shoppingCart.Id);

            if (existingProduct == null)
            {
                var productInShoppingCart = new ProductInShoppingCart
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ShoppingCartId = shoppingCart.Id,
                    Product = product,
                    ShoppingCart = shoppingCart,
                    quantity = quantity
                };

                _productInShoppingCartRepository.Insert(productInShoppingCart);
            }
            else
            {
                existingProduct.quantity += quantity;
                _productInShoppingCartRepository.Update(existingProduct);
            }
        }

        public Product DeleteById(Guid id)
        {
            var product = GetById(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            _productRepository.Delete(product);
            return product;
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll(selector: x => x).ToList();
        }

        public Product? GetById(Guid id)
        {
            return _productRepository.Get(selector: x => x,
                                           predicate: x => x.Id.Equals(id));
        }

        public AddToCartDTO GetSelectedShoppingCartProduct(Guid id)
        {
            var selectedProduct = GetById(id);

            var addProductToCartModel = new AddToCartDTO
            {
                SelectedProductId = selectedProduct!.Id,
                SelectedProductName = selectedProduct.ProductName,
                Quantity = 1
            };

            return addProductToCartModel;
        }

        public Product Insert(Product product)
        {
            product.Id = Guid.NewGuid();
            return _productRepository.Insert(product);
        }

        private ProductInShoppingCart? GetProductInShoppingCart(Guid productId, Guid cartId)
        {
            return _productInShoppingCartRepository.Get(selector: x => x,
                predicate: x => x.ShoppingCartId.ToString() == cartId.ToString()
                                                && x.ProductId.ToString() == productId.ToString());
        }

        public Product Update(Product product)
        {
            return _productRepository.Update(product);
        }
    }
}
