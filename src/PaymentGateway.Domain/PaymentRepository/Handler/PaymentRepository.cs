using PaymentGateway.Domain.PaymentRepository.Handler.Interfaces;
using PaymentGateway.Domain.PaymentRepository.Models;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.PaymentRepository.Handler
{
    public class PaymentRepository : IPaymentRepository
    {
        public async Task<PaymentRecord> Get(PaymentRecord paymentRecord)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentRecord> Upsert(PaymentRecord paymentRecord)
        {
            throw new NotImplementedException();
        }
    }
}
