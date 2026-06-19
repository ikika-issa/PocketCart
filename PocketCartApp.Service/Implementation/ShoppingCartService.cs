using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.DTO;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing.Constraints;

namespace PocketCartApp.Service.Implementation
{
    [Authorize]
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IRepository<Receipt> _receiptRepository;
        private readonly IRepository<Product> _productRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, 
            IRepository<ProductInShoppingCart> productInShoppingCartRepository, 
            IRepository<Receipt> receiptRepository,
            IRepository<Product> productRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _receiptRepository = receiptRepository;
            _productRepository = productRepository;
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

        public List<ShoppingCart> GetAll()
        {
            return _shoppingCartRepository.GetAll(selector: x => x).ToList();
        }

        public ShoppingCart? GetByUserId(string userId)
        {
            return _shoppingCartRepository.Get(selector: x => x,
                                                       predicate: x => x.CashierOnShift == userId);
        }

        public ShoppingCart Insert(ShoppingCart shoppingCart)
        {
            return _shoppingCartRepository.Insert(shoppingCart);
        }

        public ShoppingCartDTO GetByUserIdWithIncludedPrducts(string userId)
        {
            var userCart = _shoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.CashierOnShift == userId,
                include: x => x
                    .Include(z => z.ProductsInCart!)
                    .ThenInclude(p => p.Product!)
            );

            if (userCart == null || userCart.ProductsInCart == null)
            {
                return new ShoppingCartDTO
                {
                    ProductsInCart = new List<ProductInShoppingCart>(),
                    TotalPrice = 0
                };
            }

                var allProducts = userCart.ProductsInCart!;

            double totalPrice = 0.0;

            foreach (var item in allProducts)
            {
                totalPrice += item.quantity * (item.Product?.ProductPrice ?? 0);
            }

            ShoppingCartDTO model = new ShoppingCartDTO
            {
                ProductsInCart = allProducts.ToList(),
                TotalPrice = totalPrice
            };

            return model;
        }


        public ShoppingCart? GetByUserIdWithIncludedProducts(string userId)
        {
            return _shoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.CashierOnShift == userId,
                include: x => x
                    .Include(z => z.ProductsInCart)
                    .ThenInclude(p => p.Product)
            );
        }

        public bool UpdateQuantity(string userId, Guid productId, double quantity)
        {
            var cart = GetByUserIdWithIncludedProducts(userId);

            if (cart == null)
                throw new Exception("Shopping cart not found.");

            var item = cart.ProductsInCart
                .FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
                throw new Exception("Product not found in cart.");

            if (quantity > item.Product!.quantity)
            {
                return false;
            }

            if (quantity <= 0)
            {
                cart.ProductsInCart.Remove(item);
            }
            else
            {
                item.quantity = quantity;
            }

            _shoppingCartRepository.Update(cart);
            return true;
        }
        public Guid PrintReceipt(string userId)
        {
            var userCart = _shoppingCartRepository.Get(selector: x => x,
                                             predicate: x => x.CashierOnShift == userId,
                                             include: x => x.Include(z => z.ProductsInCart!).ThenInclude(m => m.Product!));

            if (userCart == null ||
                userCart.ProductsInCart == null ||
                !userCart.ProductsInCart.Any())
            {
                return Guid.Empty;
            }

            double totalPrice = 0;

            foreach (var item in userCart.ProductsInCart)
            {
                if (item.Product!.quantity < item.quantity)
                    throw new Exception($"Not enough stock for {item.Product.ProductName}.");
            }

            foreach (var item in userCart.ProductsInCart)
            {
                totalPrice +=
                    item.quantity *
                    item.Product!.ProductPrice;

                item.Product.quantity -= item.quantity;

                _productRepository.Update(item.Product);
            }

            var receipt = new Receipt
            {
                Id = Guid.NewGuid(),
                ShoppingCartId = userCart.Id,
                userId = userId,
                total = totalPrice,
                PaidAt = DateTime.UtcNow,
                currency = "MKD"
            };

            _receiptRepository.Insert(receipt);

            // PDF GENERATION
            var pdfPath = GenerateReceiptPdf(receipt, userCart);
            receipt.PdfPath = pdfPath;

            _receiptRepository.Update(receipt);

            foreach (var item in userCart.ProductsInCart.ToList())
            {
                _productInShoppingCartRepository.Delete(item);
            }

            return receipt.Id;
        }

        private string GenerateReceiptPdf(Receipt receipt, ShoppingCart shoppingCart)
        {
           string folderPath = Path.Combine(
                 Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "receipts"
                );

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = $"Receipt-{receipt.Id}.pdf";

                string filePath = Path.Combine(
                    folderPath,
                    $"Receipt-{receipt.Id}.pdf"
                );

                iTextSharp.text.Document document = new iTextSharp.text.Document();

                PdfWriter.GetInstance(
                    document,
                    new FileStream(filePath, FileMode.Create)
                );

                document.Open();

                document.Add(new Paragraph("RECEIPT"));
                document.Add(new Paragraph("-------------------"));
                document.Add(new Paragraph($"Currency: {receipt.currency}"));
                document.Add(new Paragraph($"Date: {DateTime.Now}"));
                document.Add(new Paragraph(" "));

                foreach (var item in shoppingCart.ProductsInCart!)
                {
                    double itemTotal =
                        item.quantity *
                        item.Product!.ProductPrice;

                    document.Add(
                        new Paragraph(
                            $"{item.Product.ProductName} " +
                            $"x{item.quantity} " +
                            $"- {itemTotal} {receipt.currency}"
                        )
                    );
                }

                document.Add(new Paragraph(" "));
                document.Add(
                    new Paragraph(
                        $"TOTAL: {receipt.total} {receipt.currency}"
                    )
                );

                document.Close();

            return "/receipts/" + fileName;
            }

        public ShoppingCart? GetById(Guid id)
        {
            return _shoppingCartRepository.Get(selector: x => x,
                                                       predicate: x => x.Id.Equals(id));
        }

        public ShoppingCart Update(ShoppingCart shoppingCart)
        {
            return _shoppingCartRepository.Update(shoppingCart);
        }


        public void ClearCart(string userId)
        {
            var shoppingCart = _shoppingCartRepository.Get(selector: x => x,
                                                       predicate: x => x.CashierOnShift == userId,
                                                       include: x => x.Include(z => z.ProductsInCart!));
            if (shoppingCart == null)
            {
                throw new Exception("Shopping cart not found for the user.");
            }
            

            var cartItems = _productInShoppingCartRepository.GetAll(selector: x => x,
                                                        predicate: x => x.ShoppingCartId.Equals(shoppingCart.Id)).ToList();
            foreach (var item in cartItems)
            {
                _productInShoppingCartRepository.Delete(item);
            }
        }
    }
}
