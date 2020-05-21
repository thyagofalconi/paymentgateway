using System;

namespace PaymentGateway.Model.Banking
{
    public class BankingException : Exception
    {
        public BankingException(string message) : base(message)
        {
        }
    }
}
