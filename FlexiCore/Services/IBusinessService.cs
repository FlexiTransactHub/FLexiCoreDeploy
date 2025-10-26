using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IBusinessService
    {
        Task<List<Business>> GetBusinessesByOwnerAsync(string ownerId);
        Task<Business> GetBusinessByUserIdAsync(string ownerId);
        Task CreateBusinessAsync(Business business, string ownerId);
        Task<bool> IsOwnerAsync(int businessId, string userId);
    }
}