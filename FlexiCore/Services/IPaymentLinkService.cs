using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IPaymentLinkService
    {
        Task<PaymentLink> CreatePaymentLinkAsync(PaymentLink paymentLink, string userId);
        Task<PaymentLink> GetPaymentLinkByLinkIdAsync(string linkId);
        Task<List<PaymentLink>> GetPaymentLinksByBusinessAsync(int businessId);
        Task<List<PaymentLink>> GetPaymentLinksByOwnerAsync(string ownerId);
    }
}