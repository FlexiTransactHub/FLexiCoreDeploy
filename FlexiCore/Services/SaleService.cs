using FlexiCore.Models;
using FlexiCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IBusinessRepository _businessRepository;

        public SaleService(ISaleRepository saleRepository, IBusinessRepository businessRepository)
        {
            _saleRepository = saleRepository;
            _businessRepository = businessRepository;
        }

        public async Task<List<Sale>> GetSalesByBranchAsync(int branchId)
        {
            return await _saleRepository.GetSalesByBranchAsync(branchId);
        }

        public async Task<Sale> GetSaleByIdAsync(int id)
        {
            return await _saleRepository.GetSaleByIdAsync(id);
        }

        public async Task CreateSaleAsync(Sale sale, int branchId, int employeeId, List<SaleItem> saleItems)
        {
            if (!saleItems.Any())
            {
                throw new ArgumentException("At least one sale item is required.");
            }

            if (saleItems.Any(si => si.Quantity <= 0 || si.UnitPrice < 0))
            {
                throw new ArgumentException("Sale items must have positive quantity and non-negative price.");
            }

            sale.BranchId = branchId;
            sale.EmployeeId = employeeId;
            sale.SaleDate = DateTime.UtcNow;
            sale.TotalAmount = saleItems.Sum(si => si.Quantity * si.UnitPrice);
            sale.SaleItems = saleItems;

            // Update stock for products
            foreach (var item in saleItems.Where(si => si.ProductId.HasValue))
            {
                var product = await _saleRepository.GetProductByIdAsync(item.ProductId.Value);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {item.ProductId} not found.");
                }
                if (product.StockQuantity < item.Quantity)
                {
                    throw new ArgumentException($"Insufficient stock for product {product.Name}.");
                }
                product.StockQuantity -= item.Quantity;
                await _saleRepository.UpdateProductAsync(product);
            }

            await _saleRepository.AddSaleAsync(sale);
            await _saleRepository.SaveChangesAsync();
        }

        public async Task UpdateSaleAsync(Sale sale, List<SaleItem> saleItems)
        {
            if (!saleItems.Any())
            {
                throw new ArgumentException("At least one sale item is required.");
            }

            if (saleItems.Any(si => si.Quantity <= 0 || si.UnitPrice < 0))
            {
                throw new ArgumentException("Sale items must have positive quantity and non-negative price.");
            }

            var existingSale = await _saleRepository.GetSaleByIdAsync(sale.Id);
            if (existingSale == null)
            {
                throw new ArgumentException("Sale not found.");
            }

            // Revert stock for existing sale items
            foreach (var item in existingSale.SaleItems.Where(si => si.ProductId.HasValue))
            {
                var product = await _saleRepository.GetProductByIdAsync(item.ProductId.Value);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _saleRepository.UpdateProductAsync(product);
                }
            }

            // Update sale details
            existingSale.SaleDate = sale.SaleDate;
            existingSale.BranchId = sale.BranchId;
            existingSale.EmployeeId = sale.EmployeeId;
            existingSale.TotalAmount = saleItems.Sum(si => si.Quantity * si.UnitPrice);
            existingSale.SaleItems = saleItems;

            // Update stock for new sale items
            foreach (var item in saleItems.Where(si => si.ProductId.HasValue))
            {
                var product = await _saleRepository.GetProductByIdAsync(item.ProductId.Value);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {item.ProductId} not found.");
                }
                if (product.StockQuantity < item.Quantity)
                {
                    throw new ArgumentException($"Insufficient stock for product {product.Name}.");
                }
                product.StockQuantity -= item.Quantity;
                await _saleRepository.UpdateProductAsync(product);
            }

            await _saleRepository.UpdateSaleAsync(existingSale);
            await _saleRepository.SaveChangesAsync();
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(id);
            if (sale == null)
            {
                throw new ArgumentException("Sale not found.");
            }

            // Restore stock for products
            foreach (var item in sale.SaleItems.Where(si => si.ProductId.HasValue))
            {
                var product = await _saleRepository.GetProductByIdAsync(item.ProductId.Value);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _saleRepository.UpdateProductAsync(product);
                }
            }

            await _saleRepository.DeleteSaleAsync(sale);
            await _saleRepository.SaveChangesAsync();
        }
    }
}