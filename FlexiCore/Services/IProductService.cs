using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsByBusinessAsync(int businessId);
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductsByBranchId(int businessId);
        Task<List<Service>> GetServicesByBranchId(int businessId); 
        Task CreateProductAsync(Product product, int businessId);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}