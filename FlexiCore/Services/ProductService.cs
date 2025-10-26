using FlexiCore.Models;
using FlexiCore.Repositories;
using FlexiCore.Repositories;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetProductsByBusinessAsync(int businessId)
        {
            return await _productRepository.GetProductsByBusinessAsync(businessId);
        }
        public async Task<List<Product>> GetProductsByBranchId(int businessId)
        {
            return await _productRepository.GetProductsByBusinessAsync(businessId);
        }
        public async Task<List<Service>> GetServicesByBranchId(int businessId)
        {
            return new List<Service>
            {
               new Service()
                {
                    BusinessId = 1,
                    Name = "Transport",
                    Price = 5000

                }
            };
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task CreateProductAsync(Product product, int businessId)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Product name is required.");
            }
            if (product.Price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
            if (product.StockQuantity < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative.");
            }

            product.BusinessId = businessId;
            await _productRepository.AddProductAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Product name is required.");
            }
            if (product.Price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
            if (product.StockQuantity < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative.");
            }

            await _productRepository.UpdateProductAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }

            await _productRepository.DeleteProductAsync(product);
            await _productRepository.SaveChangesAsync();
        }
    }
}