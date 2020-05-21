using FluentAssertions;
using NUnit.Framework;
using PaymentGateway.Model.PaymentRepository;
using System;
using PaymentGateway.Model.PaymentProcessing;

namespace PaymentGateway.Model.Tests
{
    public class PaymentRecordTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenANullPaymentProcessingRequest_WhenTryingToConvertToPaymentRecord_ThenItThrowsAnArgumentException()
        {
            // When

            var exception = Assert.Throws<ArgumentException>(() => PaymentRecord.Convert(null));


            // Then

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("PaymentProcessingRequest cannot be null.");
        }

        [Test]
        public void GivenAValidPaymentProcessingRequest_WhenTryingToConvertToPaymentRecord_ThenItReturnsAPaymentRecord()
        {
            // Given

            const string cardNumber = "1234567812345678";
            const int expiryMonth = 12;
            const int expiryYear = 2025;
            const int cvv = 123;
            const double amount = 123.45;
            const string currencyIsoCode = "GBP";

            var request = new PaymentProcessingRequest
            {
                CardNumber = cardNumber,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                Cvv = cvv,
                Amount = amount,
                CurrencyIsoCode = currencyIsoCode
            };

            // When

            var response = PaymentRecord.Convert(request);


            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentRecord>();
            response.CardNumber.Should().Be(cardNumber);
            response.ExpiryMonth.Should().Be(expiryMonth);
            response.ExpiryYear.Should().Be(expiryYear);
            response.Cvv.Should().Be(cvv);
            response.Amount.Should().Be(amount);
            response.CurrencyIsoCode.Should().Be(currencyIsoCode);
            response.PaymentStatus.Should().Be(PaymentStatus.Pending);
            response.PaymentGatewayId.Should().NotBe(Guid.Empty);
        }
    }
}