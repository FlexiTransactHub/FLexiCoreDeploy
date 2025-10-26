using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface ISaleService
    {
        Task<List<Sale>> GetSalesByBranchAsync(int branchId);
        Task<Sale> GetSaleByIdAsync(int id);
        Task CreateSaleAsync(Sale sale, int branchId, int employeeId, List<SaleItem> saleItems);
        Task UpdateSaleAsync(Sale sale, List<SaleItem> saleItems);
        Task DeleteSaleAsync(int id);
    }
}