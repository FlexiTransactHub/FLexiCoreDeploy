public class PaymentPageViewModel
{
    public string LinkId { get; set; }
    public decimal Amount { get; set; }
    public string Product { get; set; }
    public string Business { get; set; }
    public string PaystackKey { get; set; }
    public bool NairaEnabled { get; set; }
    public bool CryptoEnabled { get; set; }
    public string CryptoWalletAddress { get; set; }
    public string ButtonColor { get; set; }
}