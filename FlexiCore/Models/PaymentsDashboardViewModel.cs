using System.Collections.Generic;

namespace FlexiCore.Models
{
    public class PaymentsDashboardViewModel
    {
        public List<PaymentTransaction> RecentTransactions { get; set; } = new();
        public List<PaymentLink> RecentPaymentLinks { get; set; } = new();
    }
}
