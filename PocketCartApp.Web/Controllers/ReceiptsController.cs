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

        [Authorize(Roles = "Admin")]
        public IActionResult IndexAllReceipts()
        {
            return View(_receiptService.GetAll());
        }

        [Authorize(Roles = "Cashier")]
        public IActionResult IndexUserReceipts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return View(_receiptService.GetAll());
        }

        public IActionResult Details(Guid id)
        {
            var receipt = _receiptService.GetById(id);

            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }
    }
}
