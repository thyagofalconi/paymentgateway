using System;
using System.ComponentModel.DataAnnotations;
using PaymentGateway.Model.PaymentProcessing;

namespace PaymentGateway.Model.PaymentRepository
{
    public class PaymentRecord : PaymentProcessingRequest
    {
        [Key]
        public Guid PaymentGatewayId { get; set; }

        public Guid BankTransactionId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public static PaymentRecord Convert(PaymentProcessingRequest paymentProcessingRequest)
        {
            if (paymentProcessingRequest == null)
            {
                throw new ArgumentException("PaymentProcessingRequest cannot be null.");
            }

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
