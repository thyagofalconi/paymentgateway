using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NUnit.Framework;
using PaymentGateway.Model.PaymentProcessing;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace PaymentGateway.API.IntegrationTests
{
    public class PaymentGatewayApiIntegrationTests
    {
        [Test]
        public void GivenAValidPaymentRequest_WhenPostingId_ThenItReturnsASuccessfulResponse()
        {
            //Given

            var projectDir = GetProjectPath("", typeof(PaymentGatewayApiIntegrationTests).GetTypeInfo().Assembly);

            _ = new HostBuilder()
                .ConfigureWebHost(webHost =>
                    webHost
                    .UseStartup(typeof(AcquiringBank.API.Fake.Startup))
                    .UseTestServer()
                    .UseKestrel(options => options.Listen(IPAddress.Any, 51394)))
                .Start();
            
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

            HttpContent httpRequest = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            ValidateAcquirerBankSuccessfulResponse(projectDir, httpRequest);
            ValidateAcquirerBankFailedResponse(projectDir, httpRequest);
            ValidateIfFailsWhenTryingToReachApiWithoutAuthorizationHeader(projectDir, httpRequest);
        }

        private void ValidateAcquirerBankSuccessfulResponse(string projectDir, HttpContent httpRequest)
        {
            var paymentGatewayServer = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(projectDir);
                    configHost.AddJsonFile("appsettings.fakebank.successresponse.json");
                })
                .ConfigureWebHost(webHost =>
                    webHost
                        .UseStartup(typeof(Startup))
                        .UseTestServer())
                .Start();

            using var client = paymentGatewayServer.GetTestClient();
            
            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4=");

            // When

            var response = client.PostAsync("/api/PaymentProcessing/v1", httpRequest).GetAwaiter().GetResult();

            //// Then

            response.Should().NotBeNull();
            response.Content.Should().NotBeNull();

            var deserialisedResponse = JsonConvert.DeserializeObject<PaymentProcessingResponse>(response.Content.ReadAsStringAsync().Result);

            deserialisedResponse.Should().BeOfType<PaymentProcessingResponse>();
            deserialisedResponse.Success.Should().BeTrue();
            deserialisedResponse.PaymentGatewayId.Should().NotBe(Guid.Empty);
        }

        private void ValidateAcquirerBankFailedResponse(string projectDir, HttpContent httpRequest)
        {
            var paymentGatewayServer = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(projectDir);
                    configHost.AddJsonFile("appsettings.fakebank.failresponse.json");
                })
                .ConfigureWebHost(webHost =>
                    webHost
                        .UseStartup(typeof(Startup))
                        .UseTestServer())
                .Start();

            using var client = paymentGatewayServer.GetTestClient();

            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4=");

            // When

            var response = client.PostAsync("/api/PaymentProcessing/v1", httpRequest).GetAwaiter().GetResult();

            //// Then

            response.Should().NotBeNull();
            response.Content.Should().NotBeNull();

            var deserialisedResponse = JsonConvert.DeserializeObject<PaymentProcessingResponse>(response.Content.ReadAsStringAsync().Result);

            deserialisedResponse.Should().BeOfType<PaymentProcessingResponse>();
            deserialisedResponse.Success.Should().BeFalse();
            deserialisedResponse.PaymentGatewayId.Should().NotBe(Guid.Empty);
        }

        private void ValidateIfFailsWhenTryingToReachApiWithoutAuthorizationHeader(string projectDir, HttpContent httpRequest)
        {
            var paymentGatewayServer = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(projectDir);
                    configHost.AddJsonFile("appsettings.fakebank.failresponse.json");
                })
                .ConfigureWebHost(webHost =>
                    webHost
                        .UseStartup(typeof(Startup))
                        .UseTestServer())
                .Start();

            using var client = paymentGatewayServer.GetTestClient();

            // When

            var response = client.PostAsync("/api/PaymentProcessing/v1", httpRequest).GetAwaiter().GetResult();

            //// Then

            response.Should().NotBeNull();
            response.Content.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            // Get name of the target project which we want to test
            var projectName = startupAssembly.GetName().Name;

            // Get currently executing test project path
            var applicationBasePath = System.AppContext.BaseDirectory;

            // Find the path to the target project
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}