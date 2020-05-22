using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Data.Context;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.PaymentRepository;
using System;
using System.Threading.Tasks;
using PaymentGateway.Domain.DataEncryption.Interfaces;

namespace PaymentGateway.Domain.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;

        private readonly PaymentContext _context;

        private readonly IDataEncryptor _dataEncryptor;

        public PaymentRepository(ILogger<PaymentRepository> logger, PaymentContext context, IDataEncryptor dataEncryptor)
        {
            _logger = logger;
            _context = context;
            _dataEncryptor = dataEncryptor;
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
                _logger.LogError(exception.ToString());

                throw new PaymentRepositoryException(exception.Message);
            }
        }

        public async Task Upsert(PaymentRecord paymentRecord)
        {
            try
            {
                if (paymentRecord.PaymentStatus == PaymentStatus.Pending)
                {
                    paymentRecord.CardNumber = _dataEncryptor.Encrypt(paymentRecord.CardNumber);

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
                _logger.LogError(exception.ToString());

                throw new PaymentRepositoryException(exception.Message);
            }
        }
    }
}
