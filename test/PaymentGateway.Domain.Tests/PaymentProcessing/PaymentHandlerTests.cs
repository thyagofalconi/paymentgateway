using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using PaymentGateway.Domain.Banking.Interfaces;
using PaymentGateway.Domain.PaymentProcessing;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.Banking;
using PaymentGateway.Model.PaymentProcessing;
using PaymentGateway.Model.PaymentRepository;
using System;


namespace PaymentGateway.Domain.Tests.PaymentProcessing
{
    public class PaymentHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenAPaymentProcessingRequestIsProvided_WhenHandlingIt_ThenItReturnsAPaymentProcessingResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentHandler>>();
            var mockedBankingHandler = Substitute.For<IBankingHandler>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentHandler = new PaymentHandler(mockedLogger, mockedBankingHandler, mockedPaymentRepository);
            
            var request = new PaymentProcessingRequest();

            mockedBankingHandler
                .Handle(request)
                .Returns(new BankingResponse { Success = true, TransactionId = Guid.NewGuid() });

            // When

            var response = realPaymentHandler.Handle(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentProcessingResponse>();
            response.Success.Should().BeTrue();

            mockedPaymentRepository.Received(2).Upsert(Arg.Any<PaymentRecord>());
            mockedBankingHandler.Received(1).Handle(request);
        }

        [Test]
        public void GivenAPaymentProcessingRequestIsProvided_WhenHandlingItAndTheBankingReturnsAFailedTransactionResponse_ThenItReturnsAFailedPaymentProcessingResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentHandler>>();
            var mockedBankingHandler = Substitute.For<IBankingHandler>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentHandler = new PaymentHandler(mockedLogger, mockedBankingHandler, mockedPaymentRepository);

            var request = new PaymentProcessingRequest();

            mockedBankingHandler
                .Handle(request)
                .Returns(new BankingResponse { Success = false, TransactionId = Guid.NewGuid() });

            // When

            var response = realPaymentHandler.Handle(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentProcessingResponse>();
            response.Success.Should().BeFalse();

            mockedPaymentRepository.Received(2).Upsert(Arg.Any<PaymentRecord>());
            mockedBankingHandler.Received(1).Handle(request);
        }

        [Test]
        public void GivenABankingExceptionIsThrown_WhenHandlingIt_ThenItReturnsAPaymentProcessingException()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentHandler>>();
            var mockedBankingHandler = Substitute.For<IBankingHandler>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentHandler = new PaymentHandler(mockedLogger, mockedBankingHandler, mockedPaymentRepository);

            var request = new PaymentProcessingRequest();

            var exception = new BankingException("test");

            mockedBankingHandler
                .Handle(request)
                .Throws(exception);

            // When

            var response = Assert.Throws<PaymentProcessingException>(() => realPaymentHandler.Handle(request).GetAwaiter().GetResult());


            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentProcessingException>();
            response.Message.Should().Be($"There was an issue when processing the transaction with the bank. Details: {exception.Message}");

            mockedPaymentRepository.Received(1).Upsert(Arg.Any<PaymentRecord>());
            mockedBankingHandler.Received(1).Handle(request);
        }

        [Test]
        public void GivenAPaymentRepositoryExceptionIsThrown_WhenHandlingIt_ThenItReturnsAPaymentProcessingException()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentHandler>>();
            var mockedBankingHandler = Substitute.For<IBankingHandler>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentHandler = new PaymentHandler(mockedLogger, mockedBankingHandler, mockedPaymentRepository);

            var request = new PaymentProcessingRequest();

            var exception = new PaymentRepositoryException("test");

            mockedPaymentRepository
                .Upsert(Arg.Any<PaymentRecord>())
                .Throws(exception);

            // When

            var response = Assert.Throws<PaymentProcessingException>(() => realPaymentHandler.Handle(request).GetAwaiter().GetResult());


            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentProcessingException>();
            response.Message.Should().Be($"There was an issue when saving the request. Details: {exception.Message}");

            mockedPaymentRepository.Received(1).Upsert(Arg.Any<PaymentRecord>());
            mockedBankingHandler.Received(0).Handle(request);
        }

        [Test]
        public void GivenAUnhandledExceptionIsThrown_WhenHandlingIt_ThenItReturnsAPaymentProcessingException()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentHandler>>();
            var mockedBankingHandler = Substitute.For<IBankingHandler>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentHandler = new PaymentHandler(mockedLogger, mockedBankingHandler, mockedPaymentRepository);

            var request = new PaymentProcessingRequest();

            var exception = new Exception("test");

            mockedPaymentRepository
                .Upsert(Arg.Any<PaymentRecord>())
                .Throws(exception);

            // When

            var response = Assert.Throws<PaymentProcessingException>(() => realPaymentHandler.Handle(request).GetAwaiter().GetResult());


            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentProcessingException>();
            response.Message.Should().Be($"An unhandled error has occurred. Details: {exception.Message}");

            mockedPaymentRepository.Received(1).Upsert(Arg.Any<PaymentRecord>());
            mockedBankingHandler.Received(0).Handle(request);
        }
    }
}