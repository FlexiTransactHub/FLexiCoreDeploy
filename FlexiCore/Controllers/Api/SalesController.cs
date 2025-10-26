using FlexiCore.Data;
using FlexiCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiCore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] Sale sale)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            sale.SaleDate = DateTime.UtcNow;
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return Ok(sale);
        }

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetSales(int branchId)
        {
          
            return Ok();
        }
    }
}
