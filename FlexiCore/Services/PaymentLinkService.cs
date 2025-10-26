using FlexiCore.Models;
using FlexiCore.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class PaymentLinkService : IPaymentLinkService
    {
        private readonly IPaymentLinkRepository _paymentLinkRepository;

        public PaymentLinkService(IPaymentLinkRepository paymentLinkRepository)
        {
            _paymentLinkRepository = paymentLinkRepository;
        }

        public async Task<PaymentLink> CreatePaymentLinkAsync(PaymentLink paymentLink, string userId)
        {
            // Assign ownership
            paymentLink.OwnerId = userId;

            await _paymentLinkRepository.AddPaymentLinkAsync(paymentLink);
            await _paymentLinkRepository.SaveChangesAsync();

            return paymentLink;
        }

        public async Task<PaymentLink> GetPaymentLinkByLinkIdAsync(string linkId)
        {
            return await _paymentLinkRepository.GetPaymentLinkByLinkIdAsync(linkId);
        }

        public async Task<List<PaymentLink>> GetPaymentLinksByBusinessAsync(int businessId)
        {
            return await _paymentLinkRepository.GetPaymentLinksByBusinessAsync(businessId);
        }

        public async Task<List<PaymentLink>> GetPaymentLinksByOwnerAsync(string ownerId)
        {
            return await _paymentLinkRepository.GetPaymentLinksByOwnerAsync(ownerId);
        }
    }
}
