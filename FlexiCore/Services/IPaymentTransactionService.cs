using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IPaymentTransactionService
    {
        Task<PaymentTransaction> CreatePaymentTransactionAsync(
            PaymentLink paymentLink,
            string paymentMethod,
            string transactionReference,
            string customerEmail);

        Task<List<PaymentTransaction>> GetPaymentTransactionsByOwnerAsync(string ownerId);
    }
}
