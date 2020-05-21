using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using PaymentGateway.API.Controllers;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.PaymentRepository;
using PaymentGateway.Model.PaymentRetrieval;
using System;

namespace PaymentGateway.API.Tests
{
    public class PaymentRetrievalControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenAIdIsProvided_WhenTryingToGetThePaymentRecord_ThenItReturnsASuccessfulResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentRetrievalController>>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentRetrievalController = new PaymentRetrievalController(mockedLogger, mockedPaymentRepository);

            var id = Guid.NewGuid();

            var paymentRecord = new PaymentRecord
            {
                PaymentGatewayId = id
            };

            mockedPaymentRepository
                .Get(id)
                .Returns(paymentRecord);

            // When

            var response = realPaymentRetrievalController.Get(id.ToString()).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeOfType<PaymentRecord>();
            response.As<OkObjectResult>().Value.As<PaymentRecord>().PaymentGatewayId.Should().Be(id);

            mockedPaymentRepository.Received(1).Get(id);
        }

        [Test]
        public void GivenAIdThatDoesNotExistIsProvided_WhenTryingToGetThePaymentRecord_ThenItReturnsANotFoundResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentRetrievalController>>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentRetrievalController = new PaymentRetrievalController(mockedLogger, mockedPaymentRepository);
            
            var id = Guid.NewGuid();

            mockedPaymentRepository
                .Get(id)
                .Returns((PaymentRecord)null);

            // When

            var response = realPaymentRetrievalController.Get(id.ToString()).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<NotFoundResult>();

            mockedPaymentRepository.Received(1).Get(id);
        }

        [Test]
        public void GivenAnEmptyIdIsProvided_WhenTryingToPostIt_ThenItReturnsABadRequestResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentRetrievalController>>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentRetrievalController = new PaymentRetrievalController(mockedLogger, mockedPaymentRepository);

            realPaymentRetrievalController.ModelState.AddModelError("Id", "Error Message");

            // When

            var response = realPaymentRetrievalController.Get(null).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            
            mockedPaymentRepository.Received(0).Get(Arg.Any<Guid>());
        }

        [Test]
        public void GivenPaymentRepositoryThrowsAPaymentRepositoryException_WhenTryingToGetThePaymentRecord_ThenItReturnsABadRequestResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentRetrievalController>>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentRetrievalController = new PaymentRetrievalController(mockedLogger, mockedPaymentRepository);

            var exception = new PaymentRepositoryException("Test");

            mockedPaymentRepository
                .Get(Arg.Any<Guid>())
                .Throws(exception);

            // When

            var response = realPaymentRetrievalController.Get(Guid.NewGuid().ToString()).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.As<PaymentRetrievalFailedResponse>().Success.Should().BeFalse();
            response.As<BadRequestObjectResult>().Value.As<PaymentRetrievalFailedResponse>().Message.Should().Be($"An error has occurred when getting the payment record. Details: {exception.Message}");

            mockedPaymentRepository.Received(1).Get(Arg.Any<Guid>());
        }

        [Test]
        public void GivenPaymentRepositoryThrowsAnUnhandledException_WhenTryingToGetThePaymentRecord_ThenItReturnsABadRequestResponse()
        {
            // Given

            var mockedLogger = Substitute.For<ILogger<PaymentRetrievalController>>();
            var mockedPaymentRepository = Substitute.For<IPaymentRepository>();

            var realPaymentRetrievalController = new PaymentRetrievalController(mockedLogger, mockedPaymentRepository);

            var exception = new Exception("Test");

            mockedPaymentRepository
                .Get(Arg.Any<Guid>())
                .Throws(exception);

            // When

            var response = realPaymentRetrievalController.Get(Guid.NewGuid().ToString()).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.As<PaymentRetrievalFailedResponse>().Success.Should().BeFalse();
            response.As<BadRequestObjectResult>().Value.As<PaymentRetrievalFailedResponse>().Message.Should().Be($"An unhandled error has occurred. Details: {exception.Message}");

            mockedPaymentRepository.Received(1).Get(Arg.Any<Guid>());
        }
    }
}