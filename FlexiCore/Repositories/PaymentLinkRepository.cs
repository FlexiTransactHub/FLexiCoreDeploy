using FlexiCore.Data;
using FlexiCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiCore.Repositories
{
    public class PaymentLinkRepository : IPaymentLinkRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentLinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentLink> GetPaymentLinkByIdAsync(int id)
        {
            return await _context.PaymentLinks.FindAsync(id);
        }

        public async Task<PaymentLink> GetPaymentLinkByLinkIdAsync(string linkId)
        {
            return await _context.PaymentLinks
                .FirstOrDefaultAsync(pl => pl.LinkId == linkId);
        }

        public async Task<List<PaymentLink>> GetPaymentLinksByBusinessAsync(int businessId)
        {
            return await _context.PaymentLinks
                .Where(pl => pl.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<List<PaymentLink>> GetPaymentLinksByOwnerAsync(string ownerId)
        {
            return await _context.PaymentLinks
                .Where(pl => pl.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task AddPaymentLinkAsync(PaymentLink paymentLink)
        {
            await _context.PaymentLinks.AddAsync(paymentLink);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
