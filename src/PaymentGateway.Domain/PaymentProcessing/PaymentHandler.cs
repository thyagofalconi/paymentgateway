using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Banking.Interfaces;
using PaymentGateway.Domain.PaymentProcessing.Interfaces;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;
using PaymentGateway.Model.PaymentRepository;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.PaymentProcessing
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
                await _paymentRepository.Upsert(paymentRecord);

                var bankResponse = await _bankHandler.Handle(paymentProcessingRequest);

                paymentRecord.BankTransactionId = bankResponse.TransactionId;
                paymentRecord.PaymentStatus = bankResponse.Success ? PaymentStatus.Success : PaymentStatus.Failed;
                
                await _paymentRepository.Upsert(paymentRecord);
            }
            catch (BankingException bankingException)
            {
                _logger.LogError(bankingException.ToString());

                throw new PaymentProcessingException($"There was an issue when processing the transaction with the bank. Details: {bankingException.Message}");
            }
            catch (PaymentRepositoryException paymentRepositoryException)
            {
                _logger.LogError(paymentRepositoryException.ToString());

                throw new PaymentProcessingException($"There was an issue when saving the request. Details: {paymentRepositoryException.Message}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());

                throw new PaymentProcessingException($"An unhandled error has occurred. Details: {exception.Message}");
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
