using FlexiCore.Models;

namespace FlexiCore.Repositories
{
    public interface IPaymentLinkRepository
    {
        Task<PaymentLink> GetPaymentLinkByIdAsync(int id);
        Task<PaymentLink> GetPaymentLinkByLinkIdAsync(string linkId);
        Task<List<PaymentLink>> GetPaymentLinksByBusinessAsync(int businessId);
        Task<List<PaymentLink>> GetPaymentLinksByOwnerAsync(string ownerId);
        Task AddPaymentLinkAsync(PaymentLink paymentLink);
        Task SaveChangesAsync();


    }
}
