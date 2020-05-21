using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PaymentGateway.Data.Context;
using PaymentGateway.Model.PaymentRepository;
using System;

namespace PaymentGateway.Domain.Tests.PaymentRepository
{
    public class PaymentRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenAPaymentRecordIdTheExistsInTheContextIsProvided_WhenGettingIt_ThenItReturnsThePaymentRecord()
        {
            // Given

            using var localTestContext = new PaymentContext(new DbContextOptionsBuilder<PaymentContext>()
                    .UseInMemoryDatabase(databaseName: $"PaymentDatabase-Test-{Guid.NewGuid()}")
                    .Options);
            
            var mockedLogger = Substitute.For<ILogger<Domain.PaymentRepository.PaymentRepository>>();

            var realPaymentRepository = new Domain.PaymentRepository.PaymentRepository(mockedLogger, localTestContext);

            var expectedId = Guid.NewGuid();

            var expectedPaymentRecord = new PaymentRecord { PaymentGatewayId = expectedId };

            localTestContext.Payments.Add(expectedPaymentRecord);
            localTestContext.SaveChanges();
            
            // When

            var response = realPaymentRepository.Get(expectedId).GetAwaiter().GetResult();

            // Then

            response.Should().NotBeNull();
            response.Should().BeOfType<PaymentRecord>();
            response.PaymentGatewayId.Should().Be(expectedId);
        }

        [Test]
        public void GivenAPaymentRecordIdTheDoesNotExistInTheContextIsProvided_WhenGettingIt_ThenItReturnsNull()
        {
            // Given

            using var localTestContext = new PaymentContext(new DbContextOptionsBuilder<PaymentContext>()
                .UseInMemoryDatabase(databaseName: $"PaymentDatabase-Test-{Guid.NewGuid()}")
                .Options);

            var mockedLogger = Substitute.For<ILogger<Domain.PaymentRepository.PaymentRepository>>();

            var realPaymentRepository = new Domain.PaymentRepository.PaymentRepository(mockedLogger, localTestContext);

            var randomId = Guid.NewGuid();
            
            // When

            var response = realPaymentRepository.Get(randomId).GetAwaiter().GetResult();

            // Then

            response.Should().BeNull();
        }

        [Test]
        public void GivenAPendingPaymentRecordIsProvided_WhenUpsertingIt_ThenItAddsToTheContext()
        {
            // Given

            using var localTestContext = new PaymentContext(new DbContextOptionsBuilder<PaymentContext>()
                .UseInMemoryDatabase(databaseName: $"PaymentDatabase-Test-{Guid.NewGuid()}")
                .Options);

            var mockedLogger = Substitute.For<ILogger<Domain.PaymentRepository.PaymentRepository>>();

            var realPaymentRepository = new Domain.PaymentRepository.PaymentRepository(mockedLogger, localTestContext);

            var expectedId = Guid.NewGuid();
            var expectedPaymentRecord = new PaymentRecord
            {
                PaymentGatewayId = expectedId,
                PaymentStatus = PaymentStatus.Pending
            };
            
            // When

            realPaymentRepository.Upsert(expectedPaymentRecord).GetAwaiter().GetResult();

            var actualPaymentRecord = realPaymentRepository.Get(expectedId).GetAwaiter().GetResult();

            // Then

            actualPaymentRecord.Should().NotBeNull();
            actualPaymentRecord.Should().BeOfType<PaymentRecord>();
            actualPaymentRecord.PaymentGatewayId.Should().Be(expectedId);
        }

        [Test]
        public void GivenASuccessfulPaymentRecordIsProvided_WhenUpsertingIt_ThenItUpdatesTheContext()
        {
            // Given

            using var localTestContext = new PaymentContext(new DbContextOptionsBuilder<PaymentContext>()
                .UseInMemoryDatabase(databaseName: $"PaymentDatabase-Test-{Guid.NewGuid()}")
                .Options);

            var mockedLogger = Substitute.For<ILogger<Domain.PaymentRepository.PaymentRepository>>();

            var realPaymentRepository = new Domain.PaymentRepository.PaymentRepository(mockedLogger, localTestContext);

            var expectedId = Guid.NewGuid();
            var expectedPaymentRecord = new PaymentRecord
            {
                PaymentGatewayId = expectedId,
                PaymentStatus = PaymentStatus.Pending
            };

            // When

            realPaymentRepository.Upsert(expectedPaymentRecord).GetAwaiter().GetResult();

            expectedPaymentRecord.PaymentStatus = PaymentStatus.Success;

            realPaymentRepository.Upsert(expectedPaymentRecord).GetAwaiter().GetResult();

            var actualPaymentRecord = realPaymentRepository.Get(expectedId).GetAwaiter().GetResult();

            // Then

            actualPaymentRecord.Should().NotBeNull();
            actualPaymentRecord.Should().BeOfType<PaymentRecord>();
            actualPaymentRecord.PaymentGatewayId.Should().Be(expectedId);
            actualPaymentRecord.PaymentStatus.Should().Be(PaymentStatus.Success);
        }
    }
}