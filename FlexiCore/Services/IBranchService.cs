using FlexiCore.Models;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IBranchService
    {
        Task<List<Branch>> GetBranchesByBusinessAsync(int businessId);
        Task<List<Employee>> GetEmployeesByBranchId(int branchId);
        Task<List<Service>> GetServicesByBranchId(int id);
        Task<int> GetKioskEmployeeId();
        Task<Branch> GetBranchByIdAsync(int id);
        Task CreateBranchAsync(Branch branch, int businessId);
        Task UpdateBranchAsync(Branch branch);
        Task DeleteBranchAsync(int id);
    }
}