//namespace FlexiCore.Models
//{
//    public class PaymentTransaction
//    {
//    }
//}

namespace FlexiCore.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int PaymentLinkId { get; set; }
        public int BusinessId { get; set; }
        public string OwnerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // "Paystack" or "Crypto"
        public string Status { get; set; } // "Pending", "Completed", "Failed"
        public string TransactionReference { get; set; } // Paystack reference or crypto tx hash
        public DateTime CreatedAt { get; set; }
        public string CustomerEmail { get; set; }
    }
}
