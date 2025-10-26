using System.Collections.Generic;

namespace FlexiCore.Models
{
    public class DashboardViewModel
    {
        public int TotalBusinesses { get; set; }
        public int TotalBranches { get; set; }
        public int TotalProducts { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public List<Sale> RecentSales { get; set; } = new List<Sale>();
        public List<string> SalesChartLabels { get; set; } = new List<string>();
        public List<decimal> SalesChartValues { get; set; } = new List<decimal>();
    }
}