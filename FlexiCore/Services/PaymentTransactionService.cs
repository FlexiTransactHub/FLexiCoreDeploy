using FlexiCore.Models;
using FlexiCore.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;

        public PaymentTransactionService(IPaymentTransactionRepository paymentTransactionRepository)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
        }

        public async Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentLink paymentLink, string paymentMethod, string transactionReference, string customerEmail)
        {
            var transaction = new PaymentTransaction
            {
                PaymentLinkId = paymentLink.Id,
                BusinessId = paymentLink.BusinessId,
                OwnerId = paymentLink.OwnerId,
                Amount = paymentLink.Amount,
                PaymentMethod = paymentMethod,
                Status = paymentMethod == "Paystack" ? "Completed" : "Pending", // Crypto payments need manual verification
                TransactionReference = transactionReference,
                CustomerEmail = customerEmail,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentTransactionRepository.AddPaymentTransactionAsync(transaction);
            await _paymentTransactionRepository.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<PaymentTransaction>> GetPaymentTransactionsByOwnerAsync(string ownerId)
        {
            return await _paymentTransactionRepository.GetPaymentTransactionsByOwnerAsync(ownerId);
        }
    }
}