using System;

namespace PaymentGateway.Model.PaymentRepository
{
    public class PaymentRepositoryException : Exception
    {
        public PaymentRepositoryException(string message) : base(message)
        {
        }
    }
}
