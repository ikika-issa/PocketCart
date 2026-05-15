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

namespace PocketCartApp.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IRepository<Receipt> _receiptRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepository, IRepository<Receipt> receiptRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _receiptRepository = receiptRepository;
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

        public ShoppingCart? GetByUserId(Guid userId)
        {
            return _shoppingCartRepository.Get(selector: x => x,
                                                       predicate: x => x.CashierOnShift!.Equals(userId.ToString()));
        }

        public ShoppingCart Insert(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = Guid.NewGuid();
            return _shoppingCartRepository.Insert(shoppingCart);
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

        public bool PrintReceipt(Guid userId)
        {
            var userCart = _shoppingCartRepository.Get(selector: x => x,
                                             predicate: x => x.CashierOnShift!.Equals(userId.ToString()),
                                             include: x => x.Include(z => z.ProductsInCart!).ThenInclude(m => m.Product!));

            if (userCart == null ||
                userCart.ProductsInCart == null ||
                !userCart.ProductsInCart.Any())
            {
                return false;
            }

            double totalPrice = 0;

            foreach (var item in userCart.ProductsInCart)
            {
                totalPrice +=
                    item.quantity *
                    item.Product!.ProductPrice;
            }

            var receipt = new Receipt
            {
                Id = Guid.NewGuid(),
                ShoppingCartId = userCart.Id,
                total = totalPrice,
                currency = "MKD"
            };

            _receiptRepository.Insert(receipt);

            // PDF GENERATION
            GenerateReceiptPdf(receipt, userCart);

            // CLEAR CART
            userCart.ProductsInCart.Clear();
            _shoppingCartRepository.Update(userCart);

            return true;
        }

        private void GenerateReceiptPdf(Receipt receipt, ShoppingCart shoppingCart)
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
                document.Add(new Paragraph($"Receipt ID: {receipt.Id}"));
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
            }
    }
}
