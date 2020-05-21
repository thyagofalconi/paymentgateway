using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Data.Context;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.PaymentRepository;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;

        private readonly PaymentContext _context;

        public PaymentRepository(ILogger<PaymentRepository> logger, PaymentContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<PaymentRecord> Get(Guid id)
        {
            try
            {
                var paymentRecord = await _context.Payments.FirstOrDefaultAsync(i => i.PaymentGatewayId == id);

                return paymentRecord;
            }
            catch (Exception exception)
            {
                throw new PaymentRepositoryException(exception.Message);
            }
        }

        public async Task Upsert(PaymentRecord paymentRecord)
        {
            try
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
            }
            catch (Exception exception)
            {
                throw new PaymentRepositoryException(exception.Message);
            }
        }
    }
}
