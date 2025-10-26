using FlexiCore.Repositories;
using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;

        public BusinessService(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }

        public async Task<List<Business>> GetBusinessesByOwnerAsync(string ownerId)
        {
            return await _businessRepository.GetBusinessesByOwnerAsync(ownerId);
        }
        public async Task<Business> GetBusinessByUserIdAsync(string ownerId)
        {
            return null;
        }

        public async Task CreateBusinessAsync(Business business, string ownerId)
        {
            if (string.IsNullOrEmpty(business.Name) || string.IsNullOrEmpty(business.BusinessType))
            {
                throw new ArgumentException("Business name and type are required.");
            }

            business.OwnerId = ownerId;
            await _businessRepository.AddBusinessAsync(business);

            // Create default branch
            var branch = new Branch
            {
                Name = $"{business.Name} Main Branch",
                BusinessId = business.Id,
                Address = "Head Office"
            };
            await _businessRepository.AddBranchAsync(branch);

            await _businessRepository.SaveChangesAsync();
        }

        public async Task<bool> IsOwnerAsync(int businessId, string userId)
        {
            var business = await _businessRepository.GetBusinessByIdAsync(businessId);
            return business != null && business.OwnerId == userId;
        }
    }
}