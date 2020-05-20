using PaymentGateway.Domain.PaymentProcessing.Handler.Interfaces;
using PaymentGateway.Domain.PaymentProcessing.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Banking.Handler.Interfaces;
using PaymentGateway.Domain.Banking.Models;
using PaymentGateway.Domain.PaymentRepository.Handler.Interfaces;
using PaymentGateway.Domain.PaymentRepository.Models;

namespace PaymentGateway.Domain.PaymentProcessing.Handler
{
    public class PaymentHandler : IPaymentHandler
    {
        private readonly ILogger<PaymentHandler> _logger;

        private readonly IBankingHandler _bankHandler;

        private readonly IPaymentRepository _paymentRepository;

        public PaymentHandler(ILogger<PaymentHandler> logger, IBankingHandler bankHandler, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _bankHandler = bankHandler;
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentProcessingResponse> Handle(PaymentProcessingRequest paymentProcessingRequest)
        {
            var paymentRecord = PaymentRecord.Convert(paymentProcessingRequest);

            try
            {
                paymentRecord = await _paymentRepository.Upsert(paymentRecord);
            }
            catch (PaymentRepositoryException paymentRepositoryException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw;
            }

            try
            {
                var bankResponse = await _bankHandler.Handle(paymentProcessingRequest);

                if (bankResponse.TransactionStatus == TransactionStatus.Success)
                {
                    paymentRecord.BankTransactionId = bankResponse.TransactionId;

                    switch (bankResponse.TransactionStatus)
                    {
                        case TransactionStatus.Success:
                            paymentRecord.PaymentStatus = PaymentStatus.Success;
                            break;

                        default:
                            paymentRecord.PaymentStatus = PaymentStatus.Failed;
                            break;
                    }
                }

                
            }
            catch (BankingException bankingException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw;
            }
            
            try
            {
                paymentRecord = await _paymentRepository.Upsert(paymentRecord);
            }
            catch (PaymentRepositoryException paymentRepositoryException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw;
            }
            
            var response = new PaymentProcessingResponse
            {
                PaymentGatewayId = paymentRecord.PaymentGatewayId,
                Success = paymentRecord.PaymentStatus == PaymentStatus.Success
            };

            return response;
        }
    }
}
