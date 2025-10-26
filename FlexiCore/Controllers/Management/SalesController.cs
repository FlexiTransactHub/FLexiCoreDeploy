using FlexiCore.Data;
using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Management
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IBusinessService _businessService;
        private readonly IBranchService _branchService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SalesController(ISaleService saleService, IBusinessService businessService, IBranchService branchService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _saleService = saleService;
            _businessService = businessService;
            _branchService = branchService;
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? branchId)
        {
            var userId = _userManager.GetUserId(User);
            var sales = new List<Sale>();
            var branchNames = new Dictionary<int, string>();
            var businessNames = new Dictionary<int, string>();

            if (branchId.HasValue && branchId.Value > 0)
            {
                // Filter by specific branchId if provided and valid
                var branch = await _context.Branches.FindAsync(branchId.Value);
                if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
                {
                    return Unauthorized();
                }
                sales = await _saleService.GetSalesByBranchAsync(branchId.Value);
                branchNames[branchId.Value] = branch.Name;
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                var selectedBusiness = businesses.FirstOrDefault(b => b.Id == branch.BusinessId);
                if (selectedBusiness != null)
                {
                    businessNames[branch.BusinessId] = selectedBusiness.Name;
                }
                ViewData["BranchId"] = branchId.Value;
                ViewData["BusinessId"] = branch.BusinessId;
            }
            else
            {
                // Get all sales for all branches of businesses owned by the user
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                if (businesses == null || !businesses.Any())
                {
                    return View(sales); // Return empty list if no businesses
                }

                foreach (var business in businesses)
                {
                    var branches = await _branchService.GetBranchesByBusinessAsync(business.Id);
                    foreach (var branch in branches)
                    {
                        var branchSales = await _saleService.GetSalesByBranchAsync(branch.Id);
                        sales.AddRange(branchSales);
                        branchNames[branch.Id] = branch.Name;
                    }
                    businessNames[business.Id] = business.Name;
                }
            }

            ViewData["BranchNames"] = branchNames;
            ViewData["BusinessNames"] = businessNames;
            return View(sales);
        }

        public async Task<IActionResult> Create(int? branchId)
        {
            var userId = _userManager.GetUserId(User);
            Branch branch = null;
            List<Branch> branches = null;
            List<Employee> employees = new List<Employee>();
            List<Product> products = new List<Product>();
            List<Service> services = new List<Service>();

            if (branchId.HasValue && branchId != 0)
            {
                branch = await _context.Branches.FindAsync(branchId);
                if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
                {
                    return Unauthorized();
                }
                employees = await _context.Employees.Where(e => e.BranchId == branchId).ToListAsync();
                products = await _context.Products.Where(p => p.BusinessId == branch.BusinessId).ToListAsync();
                services = await _context.Services.Where(s => s.BusinessId == branch.BusinessId).ToListAsync();
            }
            else
            {
                var business = await _context.Businesses.FirstOrDefaultAsync(b => b.OwnerId == userId);
                if (business == null)
                {
                    return Unauthorized();
                }
                branches = await _branchService.GetBranchesByBusinessAsync(business.Id) ?? new List<Branch>();
                if (!branches.Any())
                {
                    return BadRequest("No branches available for this business.");
                }
            }

            ViewData["Branch"] = branch;
            ViewData["Branches"] = branches ?? new List<Branch>();
            ViewData["Employees"] = employees;
            ViewData["Products"] = products;
            ViewData["Services"] = services;

            return View(new Sale { BranchId = branchId ?? 0 });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale, int[] productIds, int[] serviceIds, int[] quantities, decimal[] unitPrices)
        {
            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(sale.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            var saleItems = new List<SaleItem>();
            for (int i = 0; i < productIds.Length; i++)
            {
                if (productIds[i] > 0)
                {
                    saleItems.Add(new SaleItem
                    {
                        ProductId = productIds[i],
                        Quantity = quantities[i],
                        UnitPrice = unitPrices[i]
                    });
                }
            }
            for (int i = 0; i < serviceIds.Length; i++)
            {
                if (serviceIds[i] > 0)
                {
                    saleItems.Add(new SaleItem
                    {
                        ServiceId = serviceIds[i],
                        Quantity = quantities[i],
                        UnitPrice = unitPrices[i]
                    });
                }
            }
                try
                {
                    await _saleService.CreateSaleAsync(sale, sale.BranchId, sale.EmployeeId, saleItems);
                    return RedirectToAction(nameof(Index), new { branchId = sale.BranchId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
           

            var employees = await _context.Employees.Where(e => e.BranchId == sale.BranchId).ToListAsync();
            var products = await _context.Products.Where(p => p.BusinessId == branch.BusinessId).ToListAsync();
            var services = await _context.Services.Where(s => s.BusinessId == branch.BusinessId).ToListAsync();

            ViewData["Employees"] = employees;
            ViewData["Products"] = products;
            ViewData["Services"] = services;

            return View(sale);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(sale.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            var employees = await _context.Employees.Where(e => e.BranchId == sale.BranchId).ToListAsync();
            var products = await _context.Products.Where(p => p.BusinessId == branch.BusinessId).ToListAsync();
            var services = await _context.Services.Where(s => s.BusinessId == branch.BusinessId).ToListAsync();

            ViewData["Employees"] = employees;
            ViewData["Products"] = products;
            ViewData["Services"] = services;

            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sale sale, int[] productIds, int[] serviceIds, int[] quantities, decimal[] unitPrices)
        {
            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(sale.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            var saleItems = new List<SaleItem>();
            for (int i = 0; i < productIds.Length; i++)
            {
                if (productIds[i] > 0)
                {
                    saleItems.Add(new SaleItem
                    {
                        ProductId = productIds[i],
                        Quantity = quantities[i],
                        UnitPrice = unitPrices[i]
                    });
                }
            }
            for (int i = 0; i < serviceIds.Length; i++)
            {
                if (serviceIds[i] > 0)
                {
                    saleItems.Add(new SaleItem
                    {
                        ServiceId = serviceIds[i],
                        Quantity = quantities[i],
                        UnitPrice = unitPrices[i]
                    });
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _saleService.UpdateSaleAsync(sale, saleItems);
                    return RedirectToAction(nameof(Index), new { branchId = sale.BranchId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var employees = await _context.Employees.Where(e => e.BranchId == sale.BranchId).ToListAsync();
            var products = await _context.Products.Where(p => p.BusinessId == branch.BusinessId).ToListAsync();
            var services = await _context.Services.Where(s => s.BusinessId == branch.BusinessId).ToListAsync();

            ViewData["Employees"] = employees;
            ViewData["Products"] = products;
            ViewData["Services"] = services;

            return View(sale);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(sale.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(sale.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            await _saleService.DeleteSaleAsync(id);
            return RedirectToAction(nameof(Index), new { branchId = sale.BranchId });
        }
    }
}