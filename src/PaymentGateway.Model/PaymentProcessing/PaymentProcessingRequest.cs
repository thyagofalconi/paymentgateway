using System;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace PaymentGateway.Model.PaymentProcessing
{
    public class PaymentProcessingRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string CardNumber { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Invalid Expiry Month Number")]
        public int ExpiryMonth { get; set; }

        [Required]
        [Range(2020, 2100, ErrorMessage = "Invalid Expiry Year Number")]
        public int ExpiryYear { get; set; }

        [Required]
        [Range(1, 999, ErrorMessage = "Invalid CVV Number")]
        public int Cvv { get; set; }

        [Required]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Invalid Amount")]
        public double Amount { get; set; }

        //https://www.iso.org/iso-4217-currency-codes.html
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CurrencyIsoCode { get; set; }
    }
}
