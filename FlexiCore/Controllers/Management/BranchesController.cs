using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Management
{
    [Authorize]
    public class BranchesController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IBusinessService _businessService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BranchesController(IBranchService branchService, IBusinessService businessService, UserManager<ApplicationUser> userManager)
        {
            _branchService = branchService;
            _businessService = businessService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? businessId)
        {
            var userId = _userManager.GetUserId(User);
            var branches = new List<Branch>();
            var businessNames = new Dictionary<int, string>();

            if (businessId.HasValue && businessId != 0)
            {
                // Filter by specific businessId if provided
                if (!await _businessService.IsOwnerAsync(businessId.Value, userId))
                {
                    return Unauthorized();
                }
                branches = await _branchService.GetBranchesByBusinessAsync(businessId.Value);
                var business = await _businessService.GetBusinessesByOwnerAsync(userId);
                var selectedBusiness = business.FirstOrDefault(b => b.Id == businessId.Value);
                if (selectedBusiness != null)
                {
                    businessNames[businessId.Value] = selectedBusiness.Name;
                }
                ViewData["BusinessId"] = businessId.Value;
            }
            else
            {
                // Get all branches for all businesses owned by the user
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                if (businesses == null || !businesses.Any())
                {
                    return View(branches); // Return empty list if no businesses
                }

                foreach (var business in businesses)
                {
                    var businessBranches = await _branchService.GetBranchesByBusinessAsync(business.Id);
                    branches.AddRange(businessBranches);
                    businessNames[business.Id] = business.Name;
                }
            }

            ViewData["BusinessNames"] = businessNames;
            return View(branches);
        }

        public async Task<IActionResult> Create(int businessId)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(businessId, userId))
            {
                return Unauthorized();
            }

            var branch = new Branch { BusinessId = businessId };
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Branch branch)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _branchService.CreateBranchAsync(branch, branch.BusinessId);
                    return RedirectToAction(nameof(Index), new { businessId = branch.BusinessId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(branch);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Branch branch)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _branchService.UpdateBranchAsync(branch);
                    return RedirectToAction(nameof(Index), new { businessId = branch.BusinessId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(branch);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            return View(branch);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            await _branchService.DeleteBranchAsync(id);
            return RedirectToAction(nameof(Index), new { businessId = branch.BusinessId });
        }
    }
}