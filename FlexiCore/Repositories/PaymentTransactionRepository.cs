using FlexiCore.Data;
using FlexiCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransaction> GetPaymentTransactionByIdAsync(int id)
        {
            return await _context.PaymentTransactions.FindAsync(id);
        }

        public async Task<List<PaymentTransaction>> GetPaymentTransactionsByBusinessAsync(int businessId)
        {
            return await _context.PaymentTransactions
                .Where(pt => pt.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<List<PaymentTransaction>> GetPaymentTransactionsByOwnerAsync(string ownerId)
        {
            return await _context.PaymentTransactions
                .Where(pt => pt.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task AddPaymentTransactionAsync(PaymentTransaction transaction)
        {
            await _context.PaymentTransactions.AddAsync(transaction);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}