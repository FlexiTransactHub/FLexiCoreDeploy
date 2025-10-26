using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(string userId);
    }
}