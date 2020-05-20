using System;

namespace PaymentGateway.Domain.PaymentProcessing.Models
{
    public class PaymentProcessingResponse
    {
        public Guid PaymentGatewayId { get; set; }

        public bool Success { get; set; }
    }
}
