using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Banking.Interfaces;
using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Banking
{
    public class BankingHandler : IBankingHandler
    {
        private readonly ILogger<BankingHandler> _logger;

        private readonly IAcquirerBankingService _acquirerBankingService;

        public BankingHandler(ILogger<BankingHandler> logger, IAcquirerBankingService acquirerBankingService)
        {
            _logger = logger;
            _acquirerBankingService = acquirerBankingService;
        }

        public async Task<BankingResponse> Handle(PaymentProcessingRequest paymentProcessingRequest)
        {
            try
            {
                var response = await _acquirerBankingService.Post(paymentProcessingRequest);

                return response;
            }
            catch (Exception exception)
            {
                throw new BankingException(exception.Message);
            }
        }
    }
}
