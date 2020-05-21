using System;

namespace AcquiringBank.API.Fake.Models
{
    public class PaymentResponse
    {
        public Guid TransactionId { get; set; }

        public bool Success { get; set; }
    }
}