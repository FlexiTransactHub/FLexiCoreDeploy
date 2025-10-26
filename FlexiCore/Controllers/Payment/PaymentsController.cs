
using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IPaymentLinkService _paymentLinkService;
        private readonly IBusinessService _businessService;
        private readonly IProductService _productService;
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentsController(
            IPaymentLinkService paymentLinkService,
            IBusinessService businessService,
            IProductService productService,
            IPaymentTransactionService paymentTransactionService)
        {
            _paymentLinkService = paymentLinkService;
            _businessService = businessService;
            _productService = productService;
            _paymentTransactionService = paymentTransactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name;

            var transactions = await _paymentTransactionService.GetPaymentTransactionsByOwnerAsync(userId);
            var links = await _paymentLinkService.GetPaymentLinksByOwnerAsync(userId);

            var model = new PaymentsDashboardViewModel
            {
                RecentTransactions = transactions.OrderByDescending(t => t.CreatedAt).Take(5).ToList(),
                RecentPaymentLinks = links.OrderByDescending(l => l.CreatedAt).Take(5).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.Identity.Name; // Assuming email as username
            var business = await _businessService.GetBusinessByUserIdAsync(userId);
            //if (business == null)
            //    return RedirectToAction("Index", "Home"); // Or handle no business case

            ViewData["Products"] = await _productService.GetProductsByBusinessAsync(2);
            ViewData["Services"] = await _productService.GetServicesByBranchId(2); // Assuming business-level services
            return View(new PaymentLink { BusinessId = 2 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentLink model, int? productId, int? serviceId)
        {
            var userId = User.Identity.Name;
            if (ModelState.IsValid)
            {
                model.ProductId = productId;
                model.ServiceId = serviceId;
                var paymentLink = await _paymentLinkService.CreatePaymentLinkAsync(model, userId);
                return View("LinkGenerated", paymentLink);
            }

            var business = await _businessService.GetBusinessByUserIdAsync(userId);
            ViewData["Products"] = await _productService.GetProductsByBusinessAsync(2);
            ViewData["Services"] = await _productService.GetServicesByBranchId(2);
            return View(model);
        }

        [HttpGet]
        public IActionResult LinkGenerated(PaymentLink paymentLink)
        {
            return View(paymentLink);
        }

        [HttpGet]
        public async Task<IActionResult> Process(string linkId)
        {
            var paymentLink = await _paymentLinkService.GetPaymentLinksByBusinessAsync(0) // Adjust to find by linkId
                .ContinueWith(t => t.Result.Find(pl => pl.LinkUrl.Contains(linkId)));
            if (paymentLink == null)
                return NotFound();

            return View(paymentLink);
        }
    }
}