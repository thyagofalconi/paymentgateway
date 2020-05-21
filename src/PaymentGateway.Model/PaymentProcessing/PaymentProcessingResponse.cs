using System;

namespace PaymentGateway.Model.PaymentProcessing
{
    public class PaymentProcessingResponse
    {
        public Guid PaymentGatewayId { get; set; }

        public bool Success { get; set; }
    }
}
