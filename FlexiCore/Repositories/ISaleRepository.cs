using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public interface ISaleRepository
    {
        Task<List<Sale>> GetSalesByBranchAsync(int branchId);
        Task<Sale> GetSaleByIdAsync(int id);
        Task AddSaleAsync(Sale sale);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Sale sale);
        Task<Product> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product);
        Task SaveChangesAsync();
    }
}