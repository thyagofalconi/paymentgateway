using AcquiringBank.API.Fake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AcquiringBank.API.Fake.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FailedPaymentController : ControllerBase
    {
        private readonly ILogger<FailedPaymentController> _logger;

        public FailedPaymentController(ILogger<FailedPaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public PaymentResponse Post([FromBody] PaymentRequest request)
        {
            return new PaymentResponse
            {
                Success = false,
                TransactionId = Guid.NewGuid()
            };
        }
    }
}
