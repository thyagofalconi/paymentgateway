using PaymentGateway.Domain.Banking.Handler.Interfaces;
using PaymentGateway.Domain.Banking.Models;
using PaymentGateway.Domain.PaymentProcessing.Models;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Banking.Handler
{
    public class BankingHandler : IBankingHandler
    {
        public Task<BankingResponse> Handle(PaymentProcessingRequest paymentProcessingRequest)
        {
            throw new NotImplementedException();
        }
    }
}
