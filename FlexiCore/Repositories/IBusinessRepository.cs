using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public interface IBusinessRepository
    {
        Task<List<Business>> GetBusinessesByOwnerAsync(string ownerId);
        Task<Business> GetBusinessByIdAsync(int id);
        Task AddBusinessAsync(Business business);
        Task AddBranchAsync(Branch branch);
        Task SaveChangesAsync();
    }
}