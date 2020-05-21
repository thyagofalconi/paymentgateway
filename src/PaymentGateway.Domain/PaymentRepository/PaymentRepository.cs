using PaymentGateway.Data.Context;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.PaymentRepository;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Domain.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentContext _context;

        public PaymentRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task<PaymentRecord> Get(Guid id)
        {
            var supplier = await _context.Payments.FirstOrDefaultAsync(i => i.PaymentGatewayId == id);

            return supplier;
        }

        public async Task<PaymentRecord> Upsert(PaymentRecord paymentRecord)
        {
            if (paymentRecord.PaymentStatus == PaymentStatus.Pending)
            {
                _context.Payments.Add(paymentRecord);
            }
            else
            {
                _context.Payments.Update(paymentRecord);
            }

            await _context.SaveChangesAsync();

            return paymentRecord;
        }
    }
}
