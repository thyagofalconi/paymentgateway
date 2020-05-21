using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;
using Refit;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Banking.Interfaces
{
    public interface IAcquirerBankingService
    {
        [Post("/")]
        Task<BankingResponse> Post(PaymentProcessingRequest paymentProcessingRequest);
    }
}