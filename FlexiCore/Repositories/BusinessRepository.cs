using FlexiCore.Data;
using FlexiCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly ApplicationDbContext _context;

        public BusinessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Business>> GetBusinessesByOwnerAsync(string ownerId)
        {
            return await _context.Businesses
                .Where(b => b.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Business> GetBusinessByIdAsync(int id)
        {
            return await _context.Businesses.FindAsync(id);
        }

        public async Task AddBusinessAsync(Business business)
        {
            await _context.Businesses.AddAsync(business);
        }

        public async Task AddBranchAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}