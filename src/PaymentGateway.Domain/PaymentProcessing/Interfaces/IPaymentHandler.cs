using System.Threading.Tasks;
using PaymentGateway.Model.PaymentProcessing;

namespace PaymentGateway.Domain.PaymentProcessing.Interfaces
{
    public interface IPaymentHandler
    {
        Task<PaymentProcessingResponse> Handle(PaymentProcessingRequest request);
    }
}