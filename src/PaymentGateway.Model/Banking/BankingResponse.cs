using System;

namespace PaymentGateway.Model.Banking
{
    public class BankingResponse
    {
        public Guid TransactionId { get; set; }

        public bool Success { get; set; }
    }
}
