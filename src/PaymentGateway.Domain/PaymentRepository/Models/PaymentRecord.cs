using PaymentGateway.Domain.PaymentProcessing.Models;
using System;

namespace PaymentGateway.Domain.PaymentRepository.Models
{
    public class PaymentRecord : PaymentProcessingRequest
    {
        public Guid PaymentGatewayId { get; set; }

        public Guid BankTransactionId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public static PaymentRecord Convert(PaymentProcessingRequest paymentProcessingRequest)
        {
            return new PaymentRecord
            {
                CardNumber = paymentProcessingRequest.CardNumber,
                ExpiryMonth = paymentProcessingRequest.ExpiryMonth,
                ExpiryYear = paymentProcessingRequest.ExpiryYear,
                Cvv = paymentProcessingRequest.Cvv,
                Amount = paymentProcessingRequest.Amount,
                CurrencyIsoCode = paymentProcessingRequest.CurrencyIsoCode,
                PaymentStatus = PaymentStatus.Pending,
                PaymentGatewayId = Guid.NewGuid()
            };
        }
    }
}
