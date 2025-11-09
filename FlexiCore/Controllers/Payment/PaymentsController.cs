
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

            // === VALIDATION ===
            //if (!ModelState.IsValid)
            //{
            //    await LoadViewDataAsync(userId);
            //    return View(model);
            //}

            // === GET BUSINESS ===
            // var business = await _businessService.GetBusinessByUserIdAsync(userId);
            // if (business == null)
            // {
            //     ModelState.AddModelError("", "Business not found.");
            //     await LoadViewDataAsync(userId);
            //     return View(model);
            // }

            // MOCKED BUSINESS
            var business = new
            {
                Id = 999,
                Name = "FlexiTransact"
            };

            model.BusinessId = business.Id;
            model.OwnerId = userId;
            model.ProductId = productId;
            model.ServiceId = serviceId;

            // === RESOLVE PRODUCT/SERVICE NAME & PRICE (MOCKED) ===
            if (productId.HasValue && productId > 0)
            {
                // Replace product DB call with mock
                var product = new { Id = 1, Name = "Website Design", Price = 50000m };
                model.Amount = product.Price;
                model.ProductName = product.Name;
            }
            else if (serviceId.HasValue && serviceId > 0)
            {
                // Replace service DB call with mock
                var service = new { Id = 101, Name = "Mobile App Development", Price = 120000m };
                model.Amount = service.Price;
                model.ProductName = service.Name;
            }
            else
            {
                // Fallback: use amount from form or default
                if (model.Amount <= 0) model.Amount = 10000m;
                model.ProductName = model.ProductName ?? "Custom Payment";
            }

            // === GENERATE UNIQUE LINK ID & URL ===
            model.LinkId = Guid.NewGuid().ToString("N")[..12]; // e.g., "a1b2c3d4e5f6"
            model.LinkUrl = Url.Action("Process", "Payments", new { linkId = model.LinkId }, Request.Scheme);
            model.CreatedAt = DateTime.UtcNow;

            // === SAVE TO DB ===
            // Note: This still calls DB to save the link
            // If you want FULL offline, mock this too (see below)
            var savedLink = await _paymentLinkService.CreatePaymentLinkAsync(model, userId);

            // === GENERATE QR CODE ===
            //using var qrGenerator = new QRCodeGenerator();
            //var qrCodeData = qrGenerator.CreateQrCode(model.LinkUrl, QRCodeGenerator.ECCLevel.Q);
            //var qrCode = new PngByteQRCode(qrCodeData);
            //byte[] qrCodeBytes = qrCode.GetGraphic(20);
            //string base64Image = Convert.ToBase64String(qrCodeBytes);
            //ViewData["QrCodeImage"] = $"data:image/png;base64,{base64Image}";

            // === PASS FULL LINK TO VIEW ===
            return View("LinkGenerated", savedLink);
        }

        [HttpGet]
        public IActionResult LinkGenerated(PaymentLink paymentLink)
        {
            return View(paymentLink);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Process(string linkId)
        {
            if (string.IsNullOrEmpty(linkId))
                return BadRequest("Invalid payment link.");

            var paymentLink = await _paymentLinkService.GetPaymentLinkByLinkIdAsync(linkId);
            if (paymentLink == null)
                return NotFound("Payment link not found or expired.");

            // Get business name
            //var business = await _businessService.GetBusinessesByOwnerAsync(paymentLink.BusinessId);
            //ViewData["BusinessName"] = business?.Name ?? "FlexiTransact";

            ViewData["BusinessName"] = "FlexiTransact";

            return View(paymentLink);
        }

        private async Task LoadViewDataAsync(string userId)
        {
            var business = await _businessService.GetBusinessByUserIdAsync(userId);
            var businessId = business?.Id ?? 0;

            ViewData["Products"] = await _productService.GetProductsByBusinessAsync(businessId);
            ViewData["Services"] = await _productService.GetServicesByBranchId(businessId);
        }
    }
}