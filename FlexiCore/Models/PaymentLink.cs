namespace FlexiCore.Models
{
    public class PaymentLink
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int? ProductId { get; set; }
        public int? ServiceId { get; set; }
        public decimal Amount { get; set; }
        public string PaystackKey { get; set; }
        public string CryptoWalletAddress { get; set; }
        public bool NairaEnabled { get; set; }
        public bool CryptoEnabled { get; set; }
        public string LinkUrl { get; set; }
        public string LinkId { get; set; } // New field for GUID
        public string OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}