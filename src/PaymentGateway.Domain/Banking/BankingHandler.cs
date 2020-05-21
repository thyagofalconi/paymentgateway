using PaymentGateway.Domain.Banking.Interfaces;
using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Banking
{
    public class BankingHandler : IBankingHandler
    {
        public Task<BankingResponse> Handle(PaymentProcessingRequest paymentProcessingRequest)
        {
            return Task.FromResult(new BankingResponse
            {
                TransactionId = Guid.NewGuid(),
                TransactionStatus = TransactionStatus.Success
            });
        }
    }
}
