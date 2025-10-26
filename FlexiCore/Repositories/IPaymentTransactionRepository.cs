using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction> GetPaymentTransactionByIdAsync(int id);
        Task<List<PaymentTransaction>> GetPaymentTransactionsByBusinessAsync(int businessId);
        Task<List<PaymentTransaction>> GetPaymentTransactionsByOwnerAsync(string ownerId);
        Task AddPaymentTransactionAsync(PaymentTransaction transaction);
        Task SaveChangesAsync();
    }
}
