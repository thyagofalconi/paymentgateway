using System;

namespace PaymentGateway.Model.PaymentProcessing
{
    public class PaymentProcessingFailedResponse
    {
        public string Message { get; set; }

        public bool Success { get; set; }
    }
}
