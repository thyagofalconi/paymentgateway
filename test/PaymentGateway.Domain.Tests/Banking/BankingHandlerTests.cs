using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using PaymentGateway.Domain.Banking;
using PaymentGateway.Domain.Banking.Interfaces;
using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;

namespace PaymentGateway.Domain.Tests.Banking
{
    public class BankingHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenAPaymentProcessingRequestIsProvided_WhenPostingToAcquirerBank_ThenItReturnsABankingResponse()
        {
            // Given

            var mockedAcquirerBankingService = Substitute.For<IAcquirerBankingService>();
            var mockedLogger = Substitute.For<ILogger<BankingHandler>>();

            var realBankingHandler = new BankingHandler(mockedLogger, mockedAcquirerBankingService);
            var request = new PaymentProcessingRequest();

            mockedAcquirerBankingService
                .Post(request)
                .Returns(new BankingResponse { Success = true, TransactionId = Guid.NewGuid() });

            // When

            var response = realBankingHandler.Handle(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BankingResponse>();
            response.Success.Should().BeTrue();
        }

        [Test]
        public void GivenAcquirerBankingServiceThrowsAnException_WhenPostingToAcquirerBank_ThenItReturnsABankingException()
        {
            // Given

            var mockedAcquirerBankingService = Substitute.For<IAcquirerBankingService>();
            var mockedLogger = Substitute.For<ILogger<BankingHandler>>();

            var realBankingHandler = new BankingHandler(mockedLogger, mockedAcquirerBankingService);
            var request = new PaymentProcessingRequest();

            var exception = new Exception("Exception thrown");

            mockedAcquirerBankingService
                .Post(request)
                .Throws(exception);

            // When

            var response = Assert.Throws<BankingException>(() => realBankingHandler.Handle(request).GetAwaiter().GetResult());

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BankingException>();
            response.Message.Should().Be(exception.Message);
        }
    }
}