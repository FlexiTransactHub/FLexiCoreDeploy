using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Management
{
    [Authorize]
    public class BusinessesController : Controller
    {
        private readonly IBusinessService _businessService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BusinessesController(IBusinessService businessService, UserManager<ApplicationUser> userManager)
        {
            _businessService = businessService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
            return View(businesses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Business business)
        {
            if (!ModelState.IsValid)
            {
                return View(business);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                await _businessService.CreateBusinessAsync(business, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(business);
            }
        }
    }
}