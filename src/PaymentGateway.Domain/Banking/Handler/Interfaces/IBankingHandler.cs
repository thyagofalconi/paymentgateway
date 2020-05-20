using PaymentGateway.Domain.Banking.Models;
using PaymentGateway.Domain.PaymentProcessing.Models;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Banking.Handler.Interfaces
{
    public interface IBankingHandler
    {
        Task<BankingResponse> Handle(PaymentProcessingRequest paymentProcessingRequest);
    }
}