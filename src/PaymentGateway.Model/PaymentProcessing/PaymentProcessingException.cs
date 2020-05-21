using System;

namespace PaymentGateway.Model.PaymentProcessing
{
    public class PaymentProcessingException : Exception
    {
        public PaymentProcessingException(string message) : base(message)
        {
        }
    }
}
