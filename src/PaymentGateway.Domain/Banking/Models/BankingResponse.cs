using System;

namespace PaymentGateway.Domain.Banking.Models
{
    public class BankingResponse
    {
        public Guid TransactionId { get; set; }

        public TransactionStatus TransactionStatus { get; set; }
    }
}
