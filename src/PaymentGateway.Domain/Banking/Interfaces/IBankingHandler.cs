using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Banking.Interfaces
{
    public interface IBankingHandler
    {
        Task<BankingResponse> Handle(PaymentProcessingRequest paymentProcessingRequest);
    }
}