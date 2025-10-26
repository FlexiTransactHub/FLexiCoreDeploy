using FlexiCore.Data;
using FlexiCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var businesses = await _context.Businesses
                .Where(b => b.OwnerId == userId)
                .ToListAsync();

            if (!businesses.Any())
            {
                // Demo data if no businesses
                return GetDemoData();
            }

            var businessIds = businesses.Select(b => b.Id).ToList();

            var totalBranches = await _context.Branches.CountAsync(br => businessIds.Contains(br.BusinessId));
            var totalProducts = await _context.Products.CountAsync(p => businessIds.Contains(p.BusinessId));
            var totalEmployees = await _context.Employees.CountAsync(e => _context.Branches.Any(br => br.Id == e.BranchId && businessIds.Contains(br.BusinessId)));
            var totalSalesAmount = await _context.Sales
                .Where(s => _context.Branches.Any(br => br.Id == s.BranchId && businessIds.Contains(br.BusinessId)))
                .SumAsync(s => s.TotalAmount);

            var recentSales = await _context.Sales
                .Include(s => s.Branch)
                .Include(s => s.Employee)
                .Where(s => _context.Branches.Any(br => br.Id == s.BranchId && businessIds.Contains(br.BusinessId)))
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToListAsync();

            // For sales chart data: daily sales for last 7 days
            var salesChartData = await GetSalesChartDataAsync(businessIds);

            return new DashboardViewModel
            {
                TotalBusinesses = businesses.Count,
                TotalBranches = totalBranches,
                TotalProducts = totalProducts,
                TotalEmployees = totalEmployees,
                TotalSalesAmount = totalSalesAmount,
                RecentSales = recentSales,
                SalesChartLabels = salesChartData.Keys.ToList(),
                SalesChartValues = salesChartData.Values.ToList()
            };
        }

        private async Task<Dictionary<string, decimal>> GetSalesChartDataAsync(List<int> businessIds)
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-6);

            var dailySales = await _context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate &&
                            _context.Branches.Any(br => br.Id == s.BranchId && businessIds.Contains(br.BusinessId)))
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(s => s.TotalAmount) })
                .ToListAsync();

            var chartData = new Dictionary<string, decimal>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var total = dailySales.FirstOrDefault(ds => ds.Date == date)?.Total ?? 0;
                chartData[date.ToString("MMM dd")] = total;
            }

            return chartData;
        }

        private DashboardViewModel GetDemoData()
        {
            return new DashboardViewModel
            {
                TotalBusinesses = 2,
                TotalBranches = 5,
                TotalProducts = 150,
                TotalEmployees = 20,
                TotalSalesAmount = 12500.50m,
                RecentSales = new List<Sale>
                {
                    new Sale { Id = 1, SaleDate = DateTime.Now.AddDays(-1), TotalAmount = 100.50m, Branch = new Branch { Name = "Main Branch" }, Employee = new Employee { Name = "John Doe" } },
                    new Sale { Id = 2, SaleDate = DateTime.Now.AddDays(-2), TotalAmount = 200.00m, Branch = new Branch { Name = "Downtown Branch" }, Employee = new Employee { Name = "Jane Smith" } }
                },
                SalesChartLabels = new List<string> { "Aug 13", "Aug 14", "Aug 15", "Aug 16", "Aug 17", "Aug 18", "Aug 19" },
                SalesChartValues = new List<decimal> { 500, 600, 700, 800, 900, 1000, 1100 }
            };
        }
    }
}