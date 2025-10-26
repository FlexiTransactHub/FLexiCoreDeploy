using FlexiCore.Data;
using FlexiCore.Models;
using FlexiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IBusinessService _businessService;
        private readonly IBranchService _branchService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeesApiController(IEmployeeService employeeService, IBusinessService businessService, IBranchService branchService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _employeeService = employeeService;
            _businessService = businessService;
            _branchService = branchService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(int? branchId)
        {
            var userId = _userManager.GetUserId(User);
            var employees = new List<Employee>();

            if (branchId.HasValue && branchId.Value > 0)
            {
                var branch = await _context.Branches.FindAsync(branchId.Value);
                if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
                {
                    return Unauthorized();
                }
                employees = await _employeeService.GetEmployeesByBranchAsync(branchId.Value);
            }
            else
            {
                var businesses = await _businessService.GetBusinessesByOwnerAsync(userId);
                if (businesses == null || !businesses.Any())
                {
                    return Ok(employees);
                }

                foreach (var business in businesses)
                {
                    var branches = await _branchService.GetBranchesByBusinessAsync(business.Id);
                    foreach (var branch in branches)
                    {
                        var branchEmployees = await _employeeService.GetEmployeesByBranchAsync(branch.Id);
                        employees.AddRange(branchEmployees);
                    }
                }
            }

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
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

            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            try
            {
                await _employeeService.CreateEmployeeAsync(employee, employee.BranchId);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);
            var branch = await _context.Branches.FindAsync(employee.BranchId);
            if (branch == null || !await _businessService.IsOwnerAsync(branch.BusinessId, userId))
            {
                return Unauthorized();
            }

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            try
            {
                await _employeeService.UpdateEmployeeAsync(employee);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
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
            return NoContent();
        }
    }
}