using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public interface IBranchRepository
    {
        Task<List<Branch>> GetBranchesByBusinessAsync(int businessId);
        Task<Branch> GetBranchByIdAsync(int id);
        Task AddBranchAsync(Branch branch);
        Task UpdateBranchAsync(Branch branch);
        Task DeleteBranchAsync(Branch branch);
        Task SaveChangesAsync();
    }
}