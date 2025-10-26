using FlexiCore.Data;
using FlexiCore.Models;
using FlexiCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;

        public SaleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sale>> GetSalesByBranchAsync(int branchId)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Service)
                .Include(s => s.Employee)
                .Where(s => s.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<Sale> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Service)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddSaleAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
        }

        public Task UpdateSaleAsync(Sale sale)
        {
            _context.Sales.Update(sale);
            return Task.CompletedTask;
        }

        public Task DeleteSaleAsync(Sale sale)
        {
            _context.Sales.Remove(sale);
            return Task.CompletedTask;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}