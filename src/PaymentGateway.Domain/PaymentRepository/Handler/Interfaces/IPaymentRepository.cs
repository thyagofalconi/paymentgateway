using System.Threading.Tasks;
using PaymentGateway.Domain.Banking.Models;
using PaymentGateway.Domain.PaymentRepository.Models;

namespace PaymentGateway.Domain.PaymentRepository.Handler.Interfaces
{
    public interface IPaymentRepository
    {
        Task<PaymentRecord> Get(PaymentRecord paymentRecord);

        Task<PaymentRecord> Upsert(PaymentRecord paymentRecord);
    }
}