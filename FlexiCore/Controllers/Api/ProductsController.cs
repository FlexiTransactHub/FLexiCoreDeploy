using FlexiCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiCore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetProducts(int businessId)
        {
            var products = await _context.Products
                .Where(p => p.BusinessId == businessId)
                .ToListAsync();
            return Ok(products);
        }
    }
}
