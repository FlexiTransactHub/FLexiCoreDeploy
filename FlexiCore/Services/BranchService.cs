using FlexiCore.Models;
using FlexiCore.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;

        public BranchService(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<List<Branch>> GetBranchesByBusinessAsync(int businessId)
        {
            return await _branchRepository.GetBranchesByBusinessAsync(businessId);
        }

        public async Task<Branch> GetBranchByIdAsync(int id)
        {
            return await _branchRepository.GetBranchByIdAsync(id);
        }
        public async Task<List<Service>> GetServicesByBranchId(int id)
        {
            return null;
        }
        public async Task<List<Employee>> GetEmployeesByBranchId(int branchId)
        {
            return new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Devan"
                }
            };
        }
        public async Task<int> GetKioskEmployeeId()
        {
            return 3;
        }
        public async Task CreateBranchAsync(Branch branch, int businessId)
        {
            if (string.IsNullOrEmpty(branch.Name))
            {
                throw new ArgumentException("Branch name is required.");
            }

            branch.BusinessId = businessId;
            await _branchRepository.AddBranchAsync(branch);
            await _branchRepository.SaveChangesAsync();
        }

        public async Task UpdateBranchAsync(Branch branch)
        {
            if (string.IsNullOrEmpty(branch.Name))
            {
                throw new ArgumentException("Branch name is required.");
            }

            await _branchRepository.UpdateBranchAsync(branch);
            await _branchRepository.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(id);
            if (branch == null)
            {
                throw new ArgumentException("Branch not found.");
            }

            await _branchRepository.DeleteBranchAsync(branch);
            await _branchRepository.SaveChangesAsync();
        }
    }
}