using System.Threading.Tasks;
using PaymentGateway.Domain.PaymentProcessing.Models;
using PaymentGateway.Domain.PaymentRepository.Models;

namespace PaymentGateway.Domain.PaymentProcessing.Handler.Interfaces
{
    public interface IPaymentHandler
    {
        Task<PaymentProcessingResponse> Handle(PaymentProcessingRequest request);
    }
}