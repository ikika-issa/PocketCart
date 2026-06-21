using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Repository;
using PocketCartApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Web.Controllers
{
    [Authorize]
    public class ReceiptsController : Controller
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Cashier"))
            {
                return View(_receiptService.GetAllByUserId(userId!));
            }
            else
            {
                return View(_receiptService.GetAll());
            }   
        }

        public IActionResult ExportCsv()
        {
            var receipts = _receiptService.GetAll();

            var csv = new StringBuilder();

            csv.AppendLine("Cashier,Date,Total,Currency");

            foreach (var r in receipts)
            {
                csv.AppendLine($"{r.PocketCartApplicationUser?.UserName}," +
                    $"{r.PaidAt:dd-MM-yyyy HH:mm},{r.total},{r.currency}");
            }

            return File(
                Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                "Receipts.csv"
            );
        }
    }
}
