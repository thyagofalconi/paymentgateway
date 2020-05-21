using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using PaymentGateway.API.Controllers;
using PaymentGateway.Domain.PaymentProcessing.Interfaces;
using PaymentGateway.Model.PaymentProcessing;
using System;

namespace PaymentGateway.API.Tests
{
    public class PaymentProcessingControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenAPaymentProcessingRequestIsProvided_WhenHandlingIt_ThenItReturnsASuccessfulResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentProcessingController>>();
            var mockedPaymentHandler = Substitute.For<IPaymentHandler>();

            var realPaymentProcessingController = new PaymentProcessingController(mockedLogger, mockedPaymentHandler);

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

            var expectedId = Guid.NewGuid();

            mockedPaymentHandler
                .Handle(request)
                .Returns(new PaymentProcessingResponse() { Success = true, PaymentGatewayId = expectedId });

            // When

            var response = realPaymentProcessingController.Post(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeOfType<PaymentProcessingResponse>();
            response.As<OkObjectResult>().Value.As<PaymentProcessingResponse>().Success.Should().BeTrue();
            response.As<OkObjectResult>().Value.As<PaymentProcessingResponse>().PaymentGatewayId.Should().Be(expectedId);

            mockedPaymentHandler.Received(1).Handle(request);
        }

        [Test]
        public void GivenAPaymentProcessingRequestIsProvidedAndPaymentHandlerReturnsAFailedTransaction_WhenHandlingIt_ThenItReturnsAFailedResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentProcessingController>>();
            var mockedPaymentHandler = Substitute.For<IPaymentHandler>();

            var realPaymentProcessingController = new PaymentProcessingController(mockedLogger, mockedPaymentHandler);

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

            var expectedId = Guid.NewGuid();

            mockedPaymentHandler
                .Handle(request)
                .Returns(new PaymentProcessingResponse() { Success = false, PaymentGatewayId = expectedId });

            // When

            var response = realPaymentProcessingController.Post(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeOfType<PaymentProcessingResponse>();
            response.As<OkObjectResult>().Value.As<PaymentProcessingResponse>().Success.Should().BeFalse();
            response.As<OkObjectResult>().Value.As<PaymentProcessingResponse>().PaymentGatewayId.Should().Be(expectedId);

            mockedPaymentHandler.Received(1).Handle(request);
        }

        [Test]
        public void GivenAnInvalidPaymentProcessingRequestIsProvided_WhenTryingToPostIt_ThenItReturnsABadRequestResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentProcessingController>>();
            var mockedPaymentHandler = Substitute.For<IPaymentHandler>();

            var realPaymentProcessingController = new PaymentProcessingController(mockedLogger, mockedPaymentHandler);

            var request = new PaymentProcessingRequest();

            realPaymentProcessingController.ModelState.AddModelError("CardNumber", "Error Message");

            // When

            var response = realPaymentProcessingController.Post(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();

            mockedPaymentHandler.Received(0).Handle(request);
        }

        [Test]
        public void GivenPaymentHandlerThrowsAPaymentProcessingException_WhenTryingToPostIt_ThenItReturnsABadRequestResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentProcessingController>>();
            var mockedPaymentHandler = Substitute.For<IPaymentHandler>();

            var realPaymentProcessingController = new PaymentProcessingController(mockedLogger, mockedPaymentHandler);

            var request = new PaymentProcessingRequest();
            
            var exception = new PaymentProcessingException("Test");

            mockedPaymentHandler
                .Handle(request)
                .Throws(exception);

            // When

            var response = realPaymentProcessingController.Post(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.As<PaymentProcessingFailedResponse>().Success.Should().BeFalse();
            response.As<BadRequestObjectResult>().Value.As<PaymentProcessingFailedResponse>().Message.Should().Be($"An error has occurred. Details: {exception.Message}");

            mockedPaymentHandler.Received(1).Handle(request);
        }

        [Test]
        public void GivenPaymentHandlerThrowsAnUnhandledException_WhenTryingToPostIt_ThenItReturnsABadRequestResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentProcessingController>>();
            var mockedPaymentHandler = Substitute.For<IPaymentHandler>();

            var realPaymentProcessingController = new PaymentProcessingController(mockedLogger, mockedPaymentHandler);

            var request = new PaymentProcessingRequest();

            var exception = new Exception("Test");

            mockedPaymentHandler
                .Handle(request)
                .Throws(exception);

            // When

            var response = realPaymentProcessingController.Post(request).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.As<PaymentProcessingFailedResponse>().Success.Should().BeFalse();
            response.As<BadRequestObjectResult>().Value.As<PaymentProcessingFailedResponse>().Message.Should().Be($"An unhandled error has occurred. Details: {exception.Message}");

            mockedPaymentHandler.Received(1).Handle(request);
        }
    }
}