using PaymentGateway.Model.PaymentRepository;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.PaymentRepository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<PaymentRecord> Get(Guid id);

        Task<PaymentRecord> Upsert(PaymentRecord paymentRecord);
    }
}