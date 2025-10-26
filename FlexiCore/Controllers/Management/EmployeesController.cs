using FlexiCore.Data;
using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Management
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IBusinessService _businessService;
        private readonly IBranchService _branchService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeesController(IEmployeeService employeeService, IBusinessService businessService, IBranchService branchService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _employeeService = employeeService;
            _businessService = businessService;
            _branchService = branchService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? branchId)
        {
            var userId = _userManager.GetUserId(User);
            var employees = new List<Employee>();
            var branchNames = new Dictionary<int, string>();
            var businessNames = new Dictionary<int, string>();

            if (branchId.HasValue && branchId.Value > 0)
            {
                var branch = await _context.Branches.FindAsync(branchId.Value);
                if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
                {
                    return Unauthorized();
                }
                employees = await _employeeService.GetEmployeesByBranchAsync(branchId.Value);
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
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                if (businesses == null || !businesses.Any())
                {
                    return View(employees);
                }

                foreach (var business in businesses)
                {
                    var branches = await _branchService.GetBranchesByBusinessAsync(business.Id);
                    foreach (var branch in branches)
                    {
                        var branchEmployees = await _employeeService.GetEmployeesByBranchAsync(branch.Id);
                        employees.AddRange(branchEmployees);
                        branchNames[branch.Id] = branch.Name;
                    }
                    businessNames[business.Id] = business.Name;
                }
            }

            ViewData["BranchNames"] = branchNames;
            ViewData["BusinessNames"] = businessNames;
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int branchId)
        {
            var userId = _userManager.GetUserId(User);

            if (branchId == 0)
            {
                // Get all businesses owned by the user
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                if (!businesses.Any())
                {
                    return Unauthorized();
                }

                // Get all branches for the user's businesses
                var branches = new List<Branch>();
                foreach (var business in businesses)
                {
                    var businessBranches = await _branchService.GetBranchesByBusinessAsync(business.Id);
                    branches.AddRange(businessBranches);
                }

                if (!branches.Any())
                {
                    return Unauthorized();
                }

                ViewData["Branches"] = new SelectList(branches, "Id", "Name");
                ViewData["IsBranchSelectable"] = true;
                ViewData["BranchId"] = null;
            }
            else
            {
                var branch = await _context.Branches.FindAsync(branchId);
                if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
                {
                    return Unauthorized();
                }

                ViewData["Branches"] = new SelectList(new[] { branch }, "Id", "Name", branchId);
                ViewData["IsBranchSelectable"] = false;
                ViewData["BranchId"] = branchId;
            }

            return View(new Employee { BranchId = branchId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            var userId = _userManager.GetUserId(User);

            // Validate BranchId explicitly
            if (employee.BranchId <= 0)
            {
                ModelState.AddModelError("BranchId", "A valid Branch ID is required.");
            }

            // Validate branch and authorization
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            employee.Branch = branch;
            employee.UserId = userId;
            employee.BranchId = branch.Id;

            // Remove navigation properties from ModelState to prevent validation issues
            ModelState.Remove("Branch");
            ModelState.Remove("BranchId");
            ModelState.Remove("Branch");
            ModelState.Remove("UserId");
            ModelState.Remove("Id");

            // Debug ModelState errors if invalid
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ModelState.AddModelError("", "Please correct the following errors: " + string.Join(", ", errors));

                ViewData["BranchId"] = employee.BranchId;
                ViewData["BranchName"] = branch?.Name ?? "Unknown";
                return View(employee);
            }

            try
            {
                await _employeeService.CreateEmployeeAsync(employee, employee.BranchId);
                return RedirectToAction(nameof(Index), new { branchId = employee.BranchId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewData["BranchId"] = employee.BranchId;
                ViewData["BranchName"] = branch?.Name ?? "Unknown";
                return View(employee);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            ViewData["BranchId"] = employee.BranchId;
            ViewData["BranchName"] = branch.Name;
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployeeAsync(employee);
                    return RedirectToAction(nameof(Index), new { branchId = employee.BranchId });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewData["BranchId"] = employee.BranchId;
            ViewData["BranchName"] = branch.Name;
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            ViewData["BranchId"] = employee.BranchId;
            ViewData["BranchName"] = branch.Name;
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            await _employeeService.DeleteEmployeeAsync(id);
            return RedirectToAction(nameof(Index), new { branchId = employee.BranchId });
        }
    }
}