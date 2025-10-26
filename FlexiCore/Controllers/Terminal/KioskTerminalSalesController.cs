using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Terminal
{
    public class KioskTerminalSalesController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;

        public KioskTerminalSalesController(
            IBranchService branchService,
            IProductService productService,
            ISaleService saleService)
        {
            _branchService = branchService;
            _productService = productService;
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int branchId)
        {
            var model = new Sale { BranchId = branchId };
            model.EmployeeId = await _branchService.GetKioskEmployeeId(); // Placeholder ID
            ViewData["Products"] = await _productService.GetProductsByBranchId(branchId);
            ViewData["Services"] = await _productService.GetServicesByBranchId(branchId);
            return View("~/Views/Terminal/Kiosk/Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale model, List<int> productIds, List<int> serviceIds, List<int> quantities, List<decimal> unitPrices)
        {
            if (ModelState.IsValid)
            {
                var saleItems = new List<SaleItem>();
                for (int i = 0; i < productIds.Count; i++)
                {
                    if (productIds[i] != 0)
                    {
                        saleItems.Add(new SaleItem { ProductId = productIds[i], Quantity = quantities[i], UnitPrice = unitPrices[i] });
                    }
                    else if (serviceIds[i] != 0)
                    {
                        saleItems.Add(new SaleItem { ServiceId = serviceIds[i], Quantity = quantities[i], UnitPrice = unitPrices[i] });
                    }
                }
                model.SaleItems = saleItems;
                await _saleService.CreateSaleAsync(model, 2, 2, saleItems);
                return RedirectToAction("Index", "Sales", new { branchId = model.BranchId });
            }

            model.EmployeeId = await _branchService.GetKioskEmployeeId();
            ViewData["Products"] = await _productService.GetProductsByBranchId(model.BranchId);
            ViewData["Services"] = await _productService.GetServicesByBranchId(model.BranchId);
            return View("~/Views/Terminal/Kiosk/Create.cshtml", model);
        }
    }
}