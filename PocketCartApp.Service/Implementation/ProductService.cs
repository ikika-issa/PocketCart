using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.Interface;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IBarcodeService _barcodeService;

        public ProductService(IRepository<Product> productRepository, 
            IRepository<ProductInShoppingCart> productInShoppingCartRepository, IShoppingCartService shoppingCartService, IBarcodeService barcodeService)
        {
            _productRepository = productRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _shoppingCartService = shoppingCartService;
            _barcodeService = barcodeService;
        }

        public void AddProductToShoppingCart(Guid id, string cashierId, int quantity)
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

        public void AddProductToShoppingCartByBarcode(string barcode, string cashierId)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                throw new Exception("Barcode cannot be empty.");
            }

            barcode = barcode.Trim();

            if (barcode.Length != 13 || !barcode.All(char.IsDigit))
            {
                throw new Exception("Invalid EAN-13 barcode format.");
            }

            var shoppingCart = _shoppingCartService.GetByUserIdWithIncludedProducts(cashierId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    Id = Guid.NewGuid(),
                    CashierOnShift = cashierId
                };

                _shoppingCartService.Insert(shoppingCart);
            }

            shoppingCart.ProductsInCart ??= new List<ProductInShoppingCart>();

            var product = GetByBarcode(barcode);

            if (product == null)
            {
                throw new Exception("Product with this barcode was not found.");
            }

            UpdateCartItem(product, shoppingCart, 1);

            _shoppingCartService.Update(shoppingCart);
        }

        private void UpdateCartItem(Product product, ShoppingCart shoppingCart, int quantity)
        {
            if (shoppingCart.ProductsInCart == null)
            {
                shoppingCart.ProductsInCart = new List<ProductInShoppingCart>();
            }

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
                                           predicate: x => x.Id == id);
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
        
        

        public byte[] ExportProducts()
        {
            var products = GetAll();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Products");

            worksheet.Cell(1, 1).Value = "Barcode";
            worksheet.Cell(1, 2).Value = "Product Name";
            worksheet.Cell(1, 3).Value = "Price";
            worksheet.Cell(1, 4).Value = "Category";
            worksheet.Cell(1, 5).Value = "Manufacturer";
            worksheet.Cell(1, 6).Value = "Expiration Date";
            worksheet.Cell(1, 7).Value = "Quantity";

            int row = 2;

            foreach (var product in products)
            {
                 worksheet.Cell(row, 1).Value = product.Barcode;
                 worksheet.Cell(row, 2).Value = product.ProductName;
                 worksheet.Cell(row, 3).Value = product.ProductPrice;
                 worksheet.Cell(row, 4).Value = product.CategoryName;
                 worksheet.Cell(row, 5).Value = product.Manufacturer?.ManufacturerName;
                 worksheet.Cell(row, 6).Value = product.ExpirationDate.ToString("dd-MM-yyyy");
                 worksheet.Cell(row, 7).Value = product.quantity;

                 row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        public Product Insert(Product product, string webRootPath)
        {
            product.Id = Guid.NewGuid();

            var barcode = _barcodeService.GenerateEAN13();
             
            product.Barcode = barcode;

            product.BarcodeImagePath = _barcodeService.GenerateBarcodeImage(barcode, webRootPath);

            return _productRepository.Insert(product);
        }

        private ProductInShoppingCart? GetProductInShoppingCart(Guid productId, Guid cartId)
        {
            return _productInShoppingCartRepository.Get(selector: x => x,
                predicate: x => x.ShoppingCartId == cartId && x.ProductId == productId);
        }

        public Product Update(Product product)
        {
            return _productRepository.Update(product);
        }

        public Product? GetByBarcode(string barcode)
        {
            barcode = barcode.Trim();

            return _productRepository.Get(
                selector: x => x,
                predicate: x => x.Barcode != null &&
                                x.Barcode.Trim() == barcode
    );
        }
    }
}
