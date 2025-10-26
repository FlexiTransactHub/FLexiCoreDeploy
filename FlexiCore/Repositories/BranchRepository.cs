using FlexiCore.Data;
using FlexiCore.Models;
using FlexiCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public BranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Branch>> GetBranchesByBusinessAsync(int businessId)
        {
            return await _context.Branches
                .Where(b => b.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(int id)
        {
            return await _context.Branches.FindAsync(id);
        }

        public async Task AddBranchAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
        }

        public Task UpdateBranchAsync(Branch branch)
        {
            _context.Branches.Update(branch);
            return Task.CompletedTask;
        }

        public Task DeleteBranchAsync(Branch branch)
        {
            _context.Branches.Remove(branch);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}