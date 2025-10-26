using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Management
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBusinessService _businessService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(IProductService productService, IBusinessService businessService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _businessService = businessService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? businessId)
        {
            var userId = _userManager.GetUserId(User);
            var products = new List<Product>();
            var businessNames = new Dictionary<int, string>();

            if (businessId.HasValue && businessId.Value > 0)
            {
                // Filter by specific businessId if provided and valid
                if (!await _businessService.IsOwnerAsync(businessId.Value, userId))
                {
                    return Unauthorized();
                }
                products = await _productService.GetProductsByBusinessAsync(businessId.Value);
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                var selectedBusiness = businesses.FirstOrDefault(b => b.Id == businessId.Value);
                if (selectedBusiness != null)
                {
                    businessNames[businessId.Value] = selectedBusiness.Name;
                }
                ViewData["BusinessId"] = businessId.Value;
            }
            else
            {
                // Get all products for all businesses owned by the user
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                if (businesses == null || !businesses.Any())
                {
                    return View(products); // Return empty list if no businesses
                }

                foreach (var business in businesses)
                {
                    var businessProducts = await _productService.GetProductsByBusinessAsync(business.Id);
                    products.AddRange(businessProducts);
                    businessNames[business.Id] = business.Name;
                }
            }

            ViewData["BusinessNames"] = businessNames;
            return View(products);
        }

        public async Task<IActionResult> Create(int businessId)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(businessId, userId))
            {
                return Unauthorized();
            }

            var product = new Product { BusinessId = businessId };
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(product.BusinessId, userId))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateProductAsync(product, product.BusinessId);
                    return RedirectToAction(nameof(Index), new { businessId = product.BusinessId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(product.BusinessId, userId))
            {
                return Unauthorized();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(product.BusinessId, userId))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product);
                    return RedirectToAction(nameof(Index), new { businessId = product.BusinessId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(product.BusinessId, userId))
            {
                return Unauthorized();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(product.BusinessId, userId))
            {
                return Unauthorized();
            }

            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index), new { businessId = product.BusinessId });
        }
    }
}